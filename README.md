# led-strip-gui
c# form to control an WS2812B LED strip using an arduino


## Serial Message Format

`{ Length, Type, Data }`

Length = number of bytes for Type + Message
Type = the instruction to carry out
Data = any data for the instruction

Messages are sent starting with their length and the arduino waits for all bytes are sent before taking action

### Message Types

Message types are ASCII values
 BRIGHTNESS = 'b'
 COLOR = 'c'
 MODE = 'm'
 PALETTE = 'p'
 UPDATES_PER_SECOND = 'u'
 PALETTE_CHANGE_DIVIDER = 'q'
 CONTROL_HUE = 'X'
 CONTROL_RGB = 'Y'
 CONTROL_HVS = 'Z'

#### Example

Sending Red color 
{ 4, 99, 255, 0, 0  }

Color message Type = c ASCII value 99
