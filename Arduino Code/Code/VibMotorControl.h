#ifndef VIBMOTORCONTROL_H
#define VIBMOTORCONTROL_H

#include <Arduino.h>

#define NUM_VIBMOTORS 14

extern const uint8_t vibMotorPins[NUM_VIBMOTORS];
extern bool vibMotorActive[NUM_VIBMOTORS];
extern unsigned long vibMotorOnTime[NUM_VIBMOTORS];
extern unsigned long vibMotorOffTime[NUM_VIBMOTORS];
extern uint8_t vibMotorIntensityPercent[NUM_VIBMOTORS];

void setupVibMotors();
void updateVibMotors();

#endif
