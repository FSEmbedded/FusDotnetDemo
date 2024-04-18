https://github.com/dotnet/iot
https://learn.microsoft.com/de-de/dotnet/iot/tutorials/blink-led

# GPIO
## Output Test
Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11 (GPIO_J1_54)
## Input Test
Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27 (GPIO_J1_52)

# CAN
## Activate CAN
Will run terminal commands to activate CAN device on local board.
External CAN receiver should be connected:
- Connect second board, CAN_L - CAN_L & CAN_H - CAN_H
- On second device , run following comand under Linux to activate can0:
    ip link set can0 up type can bitrate 1000000 && ifconfig can0 up
## Read / Write Test
Run this command on external CAN device while CAN test is running to return the received value
    STRING=$(candump can0 -L -n1 | cut -d '#' -f2) && cansend can0 01b#${STRING}

# I2C
Bus for I2C-Extension-Board must be activated in Device Tree
## Read / Write Test
Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16
## I²C Extension Board: LED Test
Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16
## I²C Extension Board: PWM / ADC Test
Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON

# SPI
Connect BBDSI with SPI: SCLK: ADP-2 -> J11-3; MOSI: ADP-3 -> J11-6; MISO: ADP-4 -> J11-5;
CS: ADP-6 -> J11-4; RESET: ADP-8 -> J11-39; GND: ADP-16 -> J11-42; +3V3: ADP-26 -> J11-1

# UART
Add NuGet-Package: system.io.port
runtimes/unix/lib/net8.0/System.IO.Ports.dll muss enthalten sein!

ttymxc0 -> uart1 -> UART_C
ttymxc1 -> uart2 -> UART_A
ttymxc2 -> uart3 -> UART_D
ttymxc3 -> uart4 -> UART_B


# PWM
Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11 (GPIO_J1_54)

# LED
## LED Blink Test
PCA9532 auf I2C-Extension-Board
sudo cp drivers/leds/leds-pca9532.ko /rootfs/home/root
Treiber für leds-pca9532 muss in Kernel und Device Tree integriert werden!
Treiber in Linux laden:
insmod leds-pca9532.ko

Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16
Default LED name for extension board: pca:red:power
Driver for PCA9532 must be enabled

# Audio
include alsa-dev to yocto release.
## Output Test
Connect Speaker to PcoreBBDSI Rev1.40 - AUDIO_A_LOUT_L - J11-49, AUDIO_A_LOUT_R - J11-45, GND - J11-47
## Input Test (Continuous)
Connect Line In to PcoreBBDSI Rev1.40 - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46
## Input Test (Fixed Time)
Connect Line In to PcoreBBDSI Rev1.40 - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46

# Camera
https://github.com/dotnet/iot/tree/ab3f910a76568d8a0c234aee0227c65705729da8/src/devices/Camera
Connect Camera to USB-Port

# Allgemein
Standartwerte für PicoCoreMX8MPr2, Rev1.10



# NuGet Packages
IoT.Device.Bindings
System.Device.Gpio


# Remote Desktop über RDP
## Keys erzeugen:
cd /etc/freerdp/keys/
openssl genrsa -out tls.key 2048
openssl req -new -key tls.key -out tls.csr
openssl x509 -req -days 365 -signkey tls.key -in tls.csr -out tls.crt
## RDP starten
/usr/bin/weston --backend=rdp-backend.so --shell=kiosk-shell.so --no-clients-resize --rdp-tls-cert=/etc/freerdp/keys/tls.crt --rdp-tls-key=/etc/freerdp/keys/tls.key
## Software auf RDP-Display ausführen
WAYLAND_DISPLAY=wayland-1 DISPLAY=:1 dotnet /home/root/IoTLib_Test/IoTLib_Test.dll

## RDP automatisch bei Boot starten
Spiegelt Display, Hardware-Display muss angeschlossen sein! -> Avalonia App startet nicht!
Eintrag in /etc/xdg/weston/weston.ini

[screen-share]
command=/usr/bin/weston --backend=rdp-backend.so --shell=fullscreen-shell.so --no-clients-resize --rdp-tls-cert=/etc/freerdp/keys/tls.crt --rdp-tls-key=/etc/freerdp/keys/tls.key
start-on-startup=true


# Remote Debugging
copy_to_board.ps1
IP anpassen, automatisch ausführen lassen

# Avalonia
UI Settings in /Views/AppStyles.axaml