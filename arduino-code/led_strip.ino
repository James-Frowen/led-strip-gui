#include <FastLED.h>

#define LED_PIN 5
#define NUM_LEDS 60
#define LED_TYPE WS2811
#define COLOR_ORDER GRB
#define UPDATES_PER_SECOND 10

// serial codes
#define S_BRIGHTNESS 98 //b
#define S_COLOR 99      //c
#define S_MODE 109      //m
#define S_PALETTE 112   //p

#define S_MODE_Manual 0
#define S_MODE_Palette 1
#define S_MODE_PeriodicPalette 2

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

CRGB leds[NUM_LEDS];

uint8_t counter = false;

uint8_t currentMode;
CRGBPalette16 currentPalette;
TBlendType currentBlending;

extern CRGBPalette16 myRedWhiteBluePalette;
extern const TProgmemPalette16 myRedWhiteBluePalette_p PROGMEM;

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

    FastLED.show();
}
void loop()
{
    checkSerial();

    if (currentMode == S_MODE_Palette || currentMode == S_MODE_PeriodicPalette)
    {
        paletteLoop();
    }

    FastLED.delay(1000 / UPDATES_PER_SECOND);
}
void checkSerial()
{
    if (Serial.available())
    {
        byte code = Serial.read();
        switch (code)
        {
        case S_BRIGHTNESS:
            setBrightness();
            break;
        case S_COLOR:
            setColor();
            break;
        case S_MODE:
            setMode();
            break;
        case S_PALETTE:
            setPattette();
            break;
        }
    }
}

void setMode()
{
    int mode = Serial.parseInt();
    currentMode = mode;
    // switch (mode)
    // {
    // case S_MODE_MANUAL:
    //     /* code */
    //     break;
    // case S_MODE_PALETTE:
    //     /* code */
    //     break;
    // case S_MODE_PeriodicPalette:
    //     /* code */
    //     break;
    // }
}
void setPattette()
{
    int palette = Serial.parseInt();

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
}
void setBrightness()
{
    int n = Serial.parseInt();
    if (n > 255)
        n = 255;
    if (n < 0)
        n = 0;
    FastLED.setBrightness(n);
}
void setColor()
{
    int r = Serial.parseInt();
    int g = Serial.parseInt();
    int b = Serial.parseInt();

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
    uint8_t secondHand = (millis() / 1000) % 60;
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
