#include "ledcontrol.h"
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

#define NUM_LEDS 14

// PWM-Controller with set I2C-Adress
Adafruit_PWMServoDriver pwm1(0x43);
Adafruit_PWMServoDriver pwm2(0x44);
Adafruit_PWMServoDriver pwm3(0x45);

// ALL LEDs default red
uint8_t ledColors[NUM_LEDS][3] = {
  {255, 0, 0}, {255, 0, 0}, {255, 0, 0}, {255, 0, 0},
  {255, 0, 0}, {255, 0, 0}, {255, 0, 0}, {255, 0, 0},
  {255, 0, 0}, {255, 0, 0}, {255, 0, 0}, {255, 0, 0},
  {255, 0, 0}, {255, 0, 0}
};

void setupLEDControllers() {
  Wire.begin();
  pwm1.begin(); pwm2.begin(); pwm3.begin();
  pwm1.setPWMFreq(1000);
  pwm2.setPWMFreq(1000);
  pwm3.setPWMFreq(1000);
}

void initializeLEDColors() {
  for (int i = 0; i < NUM_LEDS; i++) {
    setLEDColor(i, ledColors[i][0], ledColors[i][1], ledColors[i][2]);
  }
}

// Returns appropriate driver and local channel for global channel
void getPWMDriverAndChannel(int globalChannel, Adafruit_PWMServoDriver*& driver, int& localChannel) {
  if (globalChannel < 16) {
    driver = &pwm1;
    localChannel = globalChannel;
  } else if (globalChannel < 32) {
    driver = &pwm2;
    localChannel = globalChannel - 16;
  } else {
    driver = &pwm3;
    localChannel = globalChannel - 32;
  }
}

// Sets the color of an LED, continuously distributed across all drivers
void setLEDColor(int ledIndex, uint8_t red, uint8_t green, uint8_t blue) {
  int globalChannelBase = ledIndex * 3;

  // Conversion 8-bit to 12-bit PWM and inversion for common anode
  uint16_t rPWM = 4095 - (red   * 16);
  uint16_t gPWM = 4095 - (green * 16);
  uint16_t bPWM = 4095 - (blue  * 16);

  Adafruit_PWMServoDriver* driver;
  int localChannel;

  // Red
  getPWMDriverAndChannel(globalChannelBase, driver, localChannel);
  driver->setPWM(localChannel, 0, rPWM);

  // Gree
  getPWMDriverAndChannel(globalChannelBase + 1, driver, localChannel);
  driver->setPWM(localChannel, 0, gPWM);

  // Blue
  getPWMDriverAndChannel(globalChannelBase + 2, driver, localChannel);
  driver->setPWM(localChannel, 0, bPWM);
}
