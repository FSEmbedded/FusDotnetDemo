# I2C
Device Tree must activate bus for I2C-Extension-Board

# CAN
Second device:
ip link set can0 up type can bitrate 1000000 && ifconfig can0 up

has to be entered while test is already running:
STRING=$(candump can0 -L -n1 | cut -d '#' -f2) && cansend can0 01b#${STRING}

# Audio
include alsa-dev to yocto release

# Video


//TODO: Pins aufschreiben