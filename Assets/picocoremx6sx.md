# Default Pins for PicoCoreMX6SX

The connections and pins defined in this document are meant to be used with the default values that are set in the FusDotnetDemo App.
See [GPIO Reference Card](https://www.fs-net.de/assets/download/docu/PicoCore/PicoCoreMX6SX-GPIO-ReferenceCard_eng.pdf) and [Hardware Documentation](https://fs-net.de/assets/download/docu/PicoCore/PicoCoreMX6SX_Hardware_eng.pdf) for further information.
As Baseboard, *PcoreBBRGB Rev1.20* is used.


## GPIO

### Output Test

Connect LED to PcoreBBRGB - J10-3 / J10-11 (GPIO_J1_24)

### Input Test

Connect Button to PcoreBBRGB - J10-4 / J10-27 (GPIO_J1_26)


## CAN

CAN_L: PcoreBBRGB - J6-3
CAN_H: PcoreBBRGB - J6-4


## I2C

### Read / Write Test

Connect PcoreBBRGB with I²C Extension Board: I2C_B_SCL = J11-3 -> J1-11, I2C_B_SDA = J11-2 -> J1-10, GND = J11-6 -> J1-16

### I²C Extension Board: LED Test

Connect PcoreBBRGB with I²C Extension Board: I2C_B_SCL = J11-3 -> J1-11, I2C_B_SDA = J11-2 -> J1-10, GND = J11-6 -> J1-16

### I²C Extension Board: PWM / ADC Test

Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON


## SPI

Connect PcoreBBRGB with SPI-device: 
* SCLK: J10-12 -> ADP-2
* MOSI: J10-16 -> ADP-3
* MISO: J10-17 -> ADP-4
* CS: J10-14 -> ADP-6
* RESET: J10-39 -> ADP-8
* GND: J10-42 -> ADP-16
* +3V3: J10-1 -> ADP-26


## UART

ttymxc0 -> uart1 -> UART_C
ttymxc1 -> uart2 -> UART_A
ttymxc2 -> uart3 -> UART_D
ttymxc3 -> uart4 -> UART_B


## PWM

PWM Test will not work on PicoCoreMX6SX, as the GPIOs have no PWM.


## LED

Connect a keyboard with Status LEDs via USB.


## Audio

You can use either the headphone jack on the PcoreBBRGB, or use lineout with the audio pins on J11:

### Output Test

Connect Speaker to PcoreBBRGB - AUDIO_A_OUT_L - J10-49, AUDIO_A_OUT_R - J10-45, GND - J10-47

### Input Test (Continuous)

Connect Line In to PcoreBBRGB - AUDIO_A_IN_L - J10-48, AUDIO_A_IN_R - J10-44, GND - J10-46

### Input Test (Fixed Time)

Connect Line In to PcoreBBRGB - AUDIO_A_IN_L - J10-48, AUDIO_A_IN_R - J10-44, GND - J10-46


## Camera

You can use a webcam connected via USB.