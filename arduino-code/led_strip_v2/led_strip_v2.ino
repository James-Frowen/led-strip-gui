#include <FastLED.h>

// #define DEBUG
// #define ASSERT

// Debug Functions
#ifdef DEBUG
#define DEBUG_PRINT(x) Serial.print(x)
#define DEBUG_PRINTDEC(x) Serial.print(x, DEC)
#define DEBUG_PRINTLN(x) Serial.println(x)
#else
#define DEBUG_PRINT(x)
#define DEBUG_PRINTDEC(x)
#define DEBUG_PRINTLN(x)
#endif

#ifdef ASSERT
#define Assert_Equal(x, y)                                                             \
    if (x != y)                                                                        \
    {                                                                                  \
        Serial.println("Values Not Equal: '" + String(x) + "', '" + String(y) + "'."); \
    }
#define Assert_EqualM(x, y, msg)                                                              \
    if (x != y)                                                                               \
    {                                                                                         \
        Serial.println("Values Not Equal: '" + String(x) + "', '" + String(y) + "'. " + msg); \
    }
#else
#define Assert_Equal(x, y)
#define Assert_EqualM(x, y, msg)
#endif
// End Debug Functions

#define LED_PIN 5
#define NUM_LEDS 120
#define LED_TYPE WS2811
#define COLOR_ORDER GRB

// min/max settings
#define MIN_BRIGHTNESS 0
#define MAX_BRIGHTNESS 255
#define MIN_UPDATES_PER_SECOND 0
#define MAX_UPDATES_PER_SECOND 200
#define MIN_PALETTE_CHANGE_DIVIDER 500
// 1000 seconds * 60 = 1 min
#define MAX_PALETTE_CHANGE_DIVIDER 30000 // int is 16 bit, so max value is

// serial codes
#define S_BRIGHTNESS 98              //b
#define S_COLOR 99                   //c
#define S_MODE 109                   //m
#define S_PALETTE 112                //p
#define S_PALETTE_CHANGE_DIVIDER 113 //q
#define S_UPDATE_RATE 117            //u
#define S_CONTROL 88                 //X
#define S_CONTROL_RGB 89             //Y
#define S_CONTROL_HVS 90             //Z

#define S_MODE_Manual 0
#define S_MODE_Palette 1
#define S_MODE_PeriodicPalette 2
#define S_MODE_Controlled 3
#define S_MODE_LineFlash 4
// idea of modes that should be moved to Arduino
//#define S_MODE_RandomFull 5
//#define S_MODE_HueCycle 6

#define S_PALETTE_RainbowColors 0
#define S_PALETTE_RainbowStripeColors_NoBlend 1
#define S_PALETTE_RainbowStripeColors 2
#define S_PALETTE_PurpleAndGreenPalette 3
#define S_PALETTE_TotallyRandomPalette 4
#define S_PALETTE_BlackAndWhiteStripedPalette_NoBlend 5
#define S_PALETTE_BlackAndWhiteStripedPalette 6
#define S_PALETTE_CloudColors 7
#define S_PALETTE_PartyColors 8
#define S_PALETTE_RedWhiteBluePalette_NoBlend 9
#define S_PALETTE_RedWhiteBluePalette 10
// END serial codes

#define SERIAL_BUFFER_LENGTH 256

CRGB leds[NUM_LEDS];

uint8_t counter = false;

int delayCounter = 0;

int paletteChangeDivider = 1000;
int updatesPerSecond = 10;
uint8_t currentMode;
CRGBPalette16 currentPalette;
TBlendType currentBlending;

extern CRGBPalette16 myRedWhiteBluePalette;
extern const TProgmemPalette16 myRedWhiteBluePalette_p PROGMEM;

// LineFlash
const bool LineFlash_reverse = false;
const int8_t LineFlash_Length = 4;
const int8_t LineFlash_DecayLength = 8;
int LineFlash_values[NUM_LEDS];
int LineFlash_offset = 0;
bool LineFlash_forward = true;

int LineFlash_currentHue = HUE_RED;
int LineFlash_nextHue = random8();
bool LineFlash_changeCurrentHue = true;
bool LineFlash_changeNextHue = true;

/*
// Serial buffer
byte buffer[SERIAL_BUFFER_LENGTH];
int nextRead;
int nextWrite;

void WriteBuffer(byte value)
{
    buffer[nextWrite] = value;

    nextWrite++;
    if (nextWrite >= SERIAL_BUFFER_LENGTH)
    {
        nextWrite = 0;
    }
}
void WriteBuffer(byte *values, int length)
{
    for (int i = 0; i < length; i++)
    {
        WriteBuffer(values[i]);
    }
}
byte ReadBuffer()
{
    byte value = buffer[nextRead];
    nextRead++;
    if (nextRead >= SERIAL_BUFFER_LENGTH)
    {
        nextRead = 0;
    }
}
void ReadBuffer(byte *values, int length)
{
    for (int i = 0; i < length; i++)
    {
        values[i] = ReadBuffer();
    }
}
*/
void setup()
{
    Serial.begin(9600);
    delay(3000); // power-up safety delay
    FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);
    FastLED.setBrightness(4); // safe starting brightness

    currentMode = S_MODE_Manual;
    currentPalette = RainbowColors_p;
    currentBlending = LINEARBLEND;

    for (int i = 0; i < NUM_LEDS; i++)
    {
        leds[i] = CRGB::White;
    }

    setupLineFlash();

    FastLED.show();
}

void loop()
{
    // delay, at start of loop so it happens everytime
    delay(1);
    delayCounter++;

    // wait for message
    if (Serial.available() != 0)
    {
        processSerialMessage();
        return;
    }
    else
    {
        // check if update should happen
        int delayTime = 1000 / updatesPerSecond;
        if (delayCounter > delayTime)
        {
            delayCounter = 0;
            update();
        }
    }
}
void processSerialMessage()
{
    int resetAfter = 1000;
    int resetCounter = 0;

    // wait for rest of message
    byte msgLength = Serial.read();
    while (Serial.available() < msgLength)
    {
        resetCounter++;
        if (resetCounter > resetAfter)
        {
            DEBUG_PRINTLN("message length not reached");
            return;
        }
        delay(1);
    }

    // read message
    byte message[msgLength];
    for (byte i = 0; i < msgLength; i++)
    {
        message[i] = Serial.read();
    }

    // processMessage
    // Serial.println("r: ");
    // Serial.println(msgLength, DEC);
    // for (byte i = 0; i < msgLength; i++)
    // {
    //     Serial.println(message[i], DEC);
    // }
    checkSerial(message, msgLength);
}
void update()
{
    if (currentMode == S_MODE_Palette || currentMode == S_MODE_PeriodicPalette)
    {
        paletteLoop();
    }
    if (currentMode == S_MODE_LineFlash)
    {
        lineFlash();
    }
}
void checkSerial(byte *msg, int length)
{
    byte code = msg[0];
    switch (code)
    {
    case S_BRIGHTNESS:
        DEBUG_PRINTLN("S_BRIGHTNESS");
        setBrightness(msg, length);
        break;
    case S_COLOR:
        DEBUG_PRINTLN("S_COLOR");
        setColor(msg, length);
        break;
    case S_MODE:
        DEBUG_PRINTLN("S_MODE");
        setMode(msg, length);
        break;
    case S_PALETTE:
        DEBUG_PRINTLN("S_PALETTE");
        setPalette(msg, length);
        break;
    case S_PALETTE_CHANGE_DIVIDER:
        DEBUG_PRINTLN("S_PALETTE_CHANGE_DIVIDER");
        setPaletteChangeDivider(msg, length);
        break;
    case S_UPDATE_RATE:
        DEBUG_PRINTLN("S_UPDATE_RATE");
        setUpdatesPerSecond(msg, length);
        break;
    case S_CONTROL:
        DEBUG_PRINTLN("S_CONTROL");
        updateFromControl(msg, length);
        break;
    case S_CONTROL_RGB:
        DEBUG_PRINTLN("S_CONTROL_RGB");
        updateFromControlRGB(msg, length);
        break;
    case S_CONTROL_HVS:
        DEBUG_PRINTLN("S_CONTROL_HVS");
        updateFromControlHVS(msg, length);
        break;
    }
}

void setMode(byte *msg, int length)
{
    Assert_Equal(length, 2);
    int mode = msg[1];
    currentMode = mode;

#ifdef DEBUG
    DEBUG_PRINTLN(String(mode));

    if (currentMode == S_MODE_LineFlash)
    {
        for (int i = 0; i < NUM_LEDS; i++)
        {
            DEBUG_PRINT(LineFlash_values[i]);
            DEBUG_PRINT(",");
        }
        DEBUG_PRINTLN("");
    }
#endif
}
void setPalette(byte *msg, int length)
{
    Assert_Equal(length, 2);

    int palette = msg[1];

    switch (palette)
    {
    case S_PALETTE_RainbowColors:
        currentPalette = RainbowColors_p;
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_RainbowStripeColors_NoBlend:
        currentPalette = RainbowStripeColors_p;
        currentBlending = NOBLEND;
        break;
    case S_PALETTE_RainbowStripeColors:
        currentPalette = RainbowStripeColors_p;
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_PurpleAndGreenPalette:
        SetupPurpleAndGreenPalette();
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_TotallyRandomPalette:
        SetupTotallyRandomPalette();
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_BlackAndWhiteStripedPalette_NoBlend:
        SetupBlackAndWhiteStripedPalette();
        currentBlending = NOBLEND;
        break;
    case S_PALETTE_BlackAndWhiteStripedPalette:
        SetupBlackAndWhiteStripedPalette();
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_CloudColors:
        currentPalette = CloudColors_p;
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_PartyColors:
        currentPalette = PartyColors_p;
        currentBlending = LINEARBLEND;
        break;
    case S_PALETTE_RedWhiteBluePalette_NoBlend:
        currentPalette = myRedWhiteBluePalette_p;
        currentBlending = NOBLEND;
        break;
    case S_PALETTE_RedWhiteBluePalette:
        currentPalette = myRedWhiteBluePalette_p;
        currentBlending = LINEARBLEND;
        break;
    }

    DEBUG_PRINTLN(String(palette));
}
void setBrightness(byte *msg, int length)
{
    Assert_Equal(length, 2);
    int n = msg[1];
    if (n > MAX_BRIGHTNESS)
        n = MAX_BRIGHTNESS;
    if (n < MIN_BRIGHTNESS)
        n = MIN_BRIGHTNESS;
    FastLED.setBrightness(n);

    DEBUG_PRINTLN(String(n));
}
void setPaletteChangeDivider(byte *msg, int length)
{
    Assert_Equal(length, 3);

    // expecting 2 bytes
    byte b1 = msg[1];
    byte b2 = msg[2];
    int n = b2 << 8 | b1;

    if (n > MAX_PALETTE_CHANGE_DIVIDER)
        n = MAX_PALETTE_CHANGE_DIVIDER;
    if (n < MIN_PALETTE_CHANGE_DIVIDER)
        n = MIN_PALETTE_CHANGE_DIVIDER;
    paletteChangeDivider = n;

    DEBUG_PRINTLN(String(n));
}
void setUpdatesPerSecond(byte *msg, int length)
{
    Assert_Equal(length, 2);

    int n = msg[1];
    if (n > MAX_UPDATES_PER_SECOND)
        n = MAX_UPDATES_PER_SECOND;
    if (n < MIN_UPDATES_PER_SECOND)
        n = MIN_UPDATES_PER_SECOND;

    updatesPerSecond = n;

    DEBUG_PRINTLN(String(n));
}
void setColor(byte *msg, int length)
{
    Assert_Equal(length, 4);

    int r = msg[1];
    int g = msg[2];
    int b = msg[3];

    if (r > 255)
        r = 255;
    if (g > 255)
        g = 255;
    if (b > 255)
        b = 255;

    if (r < 0)
        r = 0;
    if (g < 0)
        g = 0;
    if (b < 0)
        b = 0;

    CRGB color = CRGB(r, g, b);

    for (int i = 0; i < NUM_LEDS; i++)
    {
        leds[i] = color;
    }

    FastLED.show();

    DEBUG_PRINTLN(String(r));
    DEBUG_PRINTLN(String(g));
    DEBUG_PRINTLN(String(b));
}

void updateFromControl(byte *msg, int length)
{
    Assert_Equal(length, 61);

    for (int i = 0; i < min(NUM_LEDS, 60); i++)
    {
        leds[i] = CHSV(msg[i + 1], 255, 255);
#if NUM_LEDS == 120
        leds[119 - i] = CHSV(msg[i + 1], 255, 255);
#endif
    }

    FastLED.show();
}
void updateFromControlRGB(byte *msg, int length)
{
    DEBUG_PRINTLN("updateFromControlRGB Not Implemented");
    return;
    uint8_t bytes[NUM_LEDS * 3];

    for (int i = 0; i < NUM_LEDS; i++)
    {
        uint8_t r = bytes[i * 3];
        uint8_t g = bytes[i * 3 + 1];
        uint8_t b = bytes[i * 3 + 2];
        leds[i] = CRGB(r, g, b);
#if NUM_LEDS == 120
        leds[119 - i] = CRGB(r, g, b);
#endif
    }

    FastLED.show();
}
void updateFromControlHVS(byte *msg, int length)
{
    DEBUG_PRINTLN("updateFromControlRGB Not Implemented");
    return;
    uint8_t bytes[NUM_LEDS * 3];

    for (int i = 0; i < NUM_LEDS; i++)
    {
        uint8_t h = bytes[i * 3];
        uint8_t s = bytes[i * 3 + 1];
        uint8_t b = bytes[i * 3 + 2];
        leds[i] = CHSV(h, s, b);
#if NUM_LEDS == 120
        leds[119 - i] = CHSV(h, s, b);
#endif
    }

    FastLED.show();
}

void paletteLoop()
{
    if (currentMode == S_MODE_PeriodicPalette)
    {
        ChangePalettePeriodically();
    }

    static uint8_t startIndex = 0;
    startIndex = startIndex + 1; /* motion speed */

    FillLEDsFromPaletteColors(startIndex);

    FastLED.show();
}

void FillLEDsFromPaletteColors(uint8_t colorIndex)
{
    uint8_t brightness = 255;

    for (int i = 0; i < NUM_LEDS; i++)
    {
        leds[i] = ColorFromPalette(currentPalette, colorIndex, brightness, currentBlending);
        colorIndex += 3;
    }
}

// There are several different palettes of colors demonstrated here.
//
// FastLED provides several 'preset' palettes: RainbowColors_p, RainbowStripeColors_p,
// OceanColors_p, CloudColors_p, LavaColors_p, ForestColors_p, and PartyColors_p.
//
// Additionally, you can manually define your own color palettes, or you can write
// code that creates color palettes on the fly.  All are shown here.

void ChangePalettePeriodically()
{
    uint8_t secondHand = (millis() / paletteChangeDivider) % 60;
    static uint8_t lastSecond = 99;

    if (lastSecond != secondHand)
    {
        lastSecond = secondHand;
        if (secondHand == 0)
        {
            currentPalette = RainbowColors_p;
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 10)
        {
            currentPalette = RainbowStripeColors_p;
            currentBlending = NOBLEND;
        }
        if (secondHand == 15)
        {
            currentPalette = RainbowStripeColors_p;
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 20)
        {
            SetupPurpleAndGreenPalette();
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 25)
        {
            SetupTotallyRandomPalette();
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 30)
        {
            SetupBlackAndWhiteStripedPalette();
            currentBlending = NOBLEND;
        }
        if (secondHand == 35)
        {
            SetupBlackAndWhiteStripedPalette();
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 40)
        {
            currentPalette = CloudColors_p;
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 45)
        {
            currentPalette = PartyColors_p;
            currentBlending = LINEARBLEND;
        }
        if (secondHand == 50)
        {
            currentPalette = myRedWhiteBluePalette_p;
            currentBlending = NOBLEND;
        }
        if (secondHand == 55)
        {
            currentPalette = myRedWhiteBluePalette_p;
            currentBlending = LINEARBLEND;
        }
    }
}

// This function fills the palette with totally random colors.
void SetupTotallyRandomPalette()
{
    for (int i = 0; i < 16; i++)
    {
        currentPalette[i] = CHSV(random8(), 255, random8());
    }
}

// This function sets up a palette of black and white stripes,
// using code.  Since the palette is effectively an array of
// sixteen CRGB colors, the various fill_* functions can be used
// to set them up.
void SetupBlackAndWhiteStripedPalette()
{
    // 'black out' all 16 palette entries...
    fill_solid(currentPalette, 16, CRGB::Black);
    // and set every fourth one to white.
    currentPalette[0] = CRGB::White;
    currentPalette[4] = CRGB::White;
    currentPalette[8] = CRGB::White;
    currentPalette[12] = CRGB::White;
}

// This function sets up a palette of purple and green stripes.
void SetupPurpleAndGreenPalette()
{
    CRGB purple = CHSV(HUE_PURPLE, 255, 255);
    CRGB green = CHSV(HUE_GREEN, 255, 255);
    CRGB black = CRGB::Black;

    currentPalette = CRGBPalette16(
        green, green, black, black,
        purple, purple, black, black,
        green, green, black, black,
        purple, purple, black, black);
}

// This example shows how to set up a static color palette
// which is stored in PROGMEM (flash), which is almost always more
// plentiful than RAM.  A static PROGMEM palette like this
// takes up 64 bytes of flash.
const TProgmemPalette16 myRedWhiteBluePalette_p PROGMEM =
    {
        CRGB::Red,
        CRGB::Gray, // 'white' is too bright compared to red and blue
        CRGB::Blue,
        CRGB::Black,

        CRGB::Red,
        CRGB::Gray,
        CRGB::Blue,
        CRGB::Black,

        CRGB::Red,
        CRGB::Red,
        CRGB::Gray,
        CRGB::Gray,
        CRGB::Blue,
        CRGB::Blue,
        CRGB::Black,
        CRGB::Black};

// Additionl notes on FastLED compact palettes:
//
// Normally, in computer graphics, the palette (or "color lookup table")
// has 256 entries, each containing a specific 24-bit RGB color.  You can then
// index into the color palette using a simple 8-bit (one byte) value.
// A 256-entry color palette takes up 768 bytes of RAM, which on Arduino
// is quite possibly "too many" bytes.
//
// FastLED does offer traditional 256-element palettes, for setups that
// can afford the 768-byte cost in RAM.
//
// However, FastLED also offers a compact alternative.  FastLED offers
// palettes that store 16 distinct entries, but can be accessed AS IF
// they actually have 256 entries; this is accomplished by interpolating
// between the 16 explicit entries to create fifteen intermediate palette
// entries between each pair.
//
// So for example, if you set the first two explicit entries of a compact
// palette to Green (0,255,0) and Blue (0,0,255), and then retrieved
// the first sixteen entries from the virtual palette (of 256), you'd get
// Green, followed by a smooth gradient from green-to-blue, and then Blue.

void setupLineFlash()
{
    for (int i = 0; i < LineFlash_Length; i++)
    {
        LineFlash_values[i] = 255;
    }
    for (int i = 0; i < LineFlash_DecayLength; i++)
    {
        float indexPlusOne = i + 1;
        float MaxPlusOne = LineFlash_DecayLength + 1;
        float percent = (indexPlusOne / MaxPlusOne);
        float oneMinusPercent = 1 - percent;
        oneMinusPercent = oneMinusPercent * oneMinusPercent;
        LineFlash_values[i + LineFlash_Length] = 255 * oneMinusPercent;
    }

    LineFlash_currentHue = HUE_RED;
    LineFlash_nextHue = random8();
    LineFlash_changeCurrentHue = true;
    LineFlash_changeNextHue = true;
}

void lineFlash()
{
    for (int i = 0; i < NUM_LEDS; i++)
    {
        int index = i + LineFlash_offset;
        if (index > NUM_LEDS)
        {
            index -= NUM_LEDS;
        }

        int hue = i < NUM_LEDS / 2
                      ? LineFlash_currentHue
                      : LineFlash_nextHue;

        leds[i] = CHSV(hue, 255, LineFlash_values[index]);
    }
    FastLED.show();

    LineFlash_offset++;
    DEBUG_PRINTLN(LineFlash_offset);
    if (LineFlash_offset > NUM_LEDS)
    {
        DEBUG_PRINTLN("LineFlash_offset reset");

        LineFlash_offset -= NUM_LEDS;
        LineFlash_changeCurrentHue = true;
        LineFlash_changeNextHue = true;
    }

    if (LineFlash_changeCurrentHue == true)
    {
        if (LineFlash_offset > NUM_LEDS / 4)
        {
            LineFlash_currentHue = LineFlash_nextHue;
            LineFlash_changeCurrentHue = false;
            DEBUG_PRINTLN("change CurrentHue");
            DEBUG_PRINTLN(String(LineFlash_changeCurrentHue));
        }
    }
    if (LineFlash_changeNextHue == true)
    {
        if (LineFlash_offset > 3 * NUM_LEDS / 4)
        {
            LineFlash_nextHue = random8();
            LineFlash_changeNextHue = false;
            DEBUG_PRINTLN("change LineFlash_nextHue");
            DEBUG_PRINTLN(String(LineFlash_changeNextHue));
        }
    }
}