# I2C
Device Tree must activate bus for I2C-Extension-Board

# CAN
Second device:
ip link set can0 up type can bitrate 1000000 && ifconfig can0 up

has to be entered while test is already running:
STRING=$(candump can0 -L -n1 | cut -d '#' -f2) && cansend can0 01b#${STRING}

# Audio
include alsa-dev to yocto release

Standartwerte für PicoCoreMX8MPr2, Rev1.10

# NuGet Packages
IoT.Device.Bindings
System.Device.Gpio

# Video
https://github.com/dotnet/iot/tree/ab3f910a76568d8a0c234aee0227c65705729da8/src/devices/Camera

# ADC
ADS7828 auf I2C-Extension-Board
Treiber für ads7828 muss in Kernel und Device Tree integriert werden!
Treiber in Linux laden:
insmod ads7828.ko

cat ./sys/bus/i2c/drivers/ads7828/5-004b/hwmon/hwmon1/in0_input

# LED
PCA9532 auf I2C-Extension-Board
Treiber für leds-pca9532 muss in Kernel und Device Tree integriert werden!
Treiber in Linux laden:
insmod leds-pca9532.ko


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



//TODO: Pins aufschreiben