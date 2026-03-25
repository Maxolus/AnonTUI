#ifndef BUTTONS_H
#define BUTTONS_H

#include <Arduino.h>

#define NUM_BUTTONS 14
#define DEBOUNCE_DELAY 20  // in ms

extern const uint8_t buttonPins[NUM_BUTTONS];
extern volatile uint8_t buttonStates[NUM_BUTTONS]; // 1 = pressed, 0 = not pressed

void initButtons();
void updateButtons();

#endif
