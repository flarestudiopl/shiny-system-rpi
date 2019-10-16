#!/bin/sh

sudo timedatectl set-ntp 0
sudo timedatectl set-timezone Europe/Warsaw
sudo modprobe i2c-bcm2708
sudo modprobe ds2482
sudo chmod 777 /sys/bus/i2c/devices/i2c-1/new_device
sudo echo ds2482 0x18 > /sys/bus/i2c/devices/i2c-1/new_device
sudo setcap CAP_NET_BIND_SERVICE=+eip HeatingApi

export ASPNETCORE_ENVIRONMENT=rpi
./HeatingApi
