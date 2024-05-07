# Default Pins for PicoCoreMX8MPr2

The connections and pins defined in this document are meant to be used with the default values that are set in the FusDotnetDemo App.


## GPIO

### Output Test

Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11 (GPIO_J1_54)

### Input Test

Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27 (GPIO_J1_52)


## CAN

CAN_L: PcoreBBDSI Rev1.40 - J7-3
CAN_H: PcoreBBDSI Rev1.40 - J7-4


## I2C

### Read / Write Test

Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16

### I²C Extension Board: LED Test

Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16

### I²C Extension Board: PWM / ADC Test

Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON


## SPI

Connect BBDSI with SPI-device: 
* SCLK: J11-3 -> ADP-2
* MOSI: J11-6 -> ADP-3
* MISO: J11-5 -> ADP-4
* CS: J11-4 -> ADP-6
* RESET: J11-39 -> ADP-8
* GND: J11-42 -> ADP-16
* +3V3: J11-1 -> ADP-26


## UART

ttymxc0 -> uart1 -> UART_C
ttymxc1 -> uart2 -> UART_A
ttymxc2 -> uart3 -> UART_D
ttymxc3 -> uart4 -> UART_B


## PWM

Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11 (GPIO_J1_54)


## LED

Connect a keyboard with CapsLock LED via USB.


## Audio

You can use either the headphone jack on the BBDSI, or use lineout with the audio pins on J11:

### Output Test

Connect Speaker to PcoreBBDSI Rev1.40 - AUDIO_A_LOUT_L - J11-49, AUDIO_A_LOUT_R - J11-45, GND - J11-47

### Input Test (Continuous)

Connect Line In to PcoreBBDSI Rev1.40 - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46

### Input Test (Fixed Time)

Connect Line In to PcoreBBDSI Rev1.40 - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46


## Camera

You can use a webcam connected via USB.