#ifndef LEDCONTROL_H
#define LEDCONTROL_H

#include <Arduino.h>
#include <Adafruit_PWMServoDriver.h>
#include <Wire.h>

#define NUM_LEDS 14

extern Adafruit_PWMServoDriver pwm1;
extern Adafruit_PWMServoDriver pwm2;
extern Adafruit_PWMServoDriver pwm3;

extern uint8_t ledColors[NUM_LEDS][3];

void setupLEDControllers();
void initializeLEDColors();
void setLEDColor(int ledIndex, uint8_t red, uint8_t green, uint8_t blue);

#endif
