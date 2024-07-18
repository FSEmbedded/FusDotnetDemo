# Default Pins for EfusA9X

The connections and pins defined in this document are meant to be used with the default values that are set in the FusDotnetDemo App.
See [GPIO Reference Card](https://fs-net.de/assets/download/docu/efus/efusA9X-GPIO-ReferenceCard_eng.pdf) and [Hardware Documentation](https://fs-net.de/assets/download/docu/efus/efusA9X_Hardware_eng.pdf) for further information.
As Baseboard, *EFUS-SINTF Rev1.50* is used.


## GPIO

### Output Test

//TODO

### Input Test

//TODO


## CAN

CAN_L: EFUS-SINTF - J13-3
CAN_H: EFUS-SINTF - J13-4


## I2C

### Read / Write Test

//TODO Connect EFUS-SINTF with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16

### I²C Extension Board: LED Test

//TODO Connect EFUS-SINTF with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16

### I²C Extension Board: PWM / ADC Test

//TODO Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON


## SPI

//TODO
Connect PcoreBBDSI with SPI-device: 
* SCLK: J11-3 -> ADP-2
* MOSI: J11-6 -> ADP-3
* MISO: J11-5 -> ADP-4
* CS: J11-4 -> ADP-6
* RESET: J11-39 -> ADP-8
* GND: J11-42 -> ADP-16
* +3V3: J11-1 -> ADP-26


## UART

//TODO
ttymxc0 -> uart1 -> UART_C
ttymxc1 -> uart2 -> UART_A
ttymxc2 -> uart3 -> UART_D
ttymxc3 -> uart4 -> UART_B


## PWM

//TODO
Connect LED to PcoreBBDSI - J11-8 / J11-11 (GPIO_J1_54) (Same as in GPIO Test)


## LED

Connect a keyboard with Status LEDs via USB.


## Audio

//TODO
You can use either the headphone jack on the PcoreBBDSI, or use lineout with the audio pins on J11:

### Output Test

//TODO Connect Speaker to PcoreBBDSI - AUDIO_A_LOUT_L - J11-49, AUDIO_A_LOUT_R - J11-45, GND - J11-47

### Input Test (Continuous)

//TODO Connect Line In to PcoreBBDSI - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46

### Input Test (Fixed Time)

//TODO Connect Line In to PcoreBBDSI - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46


## Camera

//TODO You can use a webcam connected via USB.