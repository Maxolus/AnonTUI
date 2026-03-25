#include "buttons.h"

const uint8_t buttonPins[NUM_BUTTONS] = {
  22, 23, 24, 25, 26, 27, 28, 29,
  30, 31, 32, 33, 34, 35
};

volatile uint8_t buttonStates[NUM_BUTTONS] = {0};

// internal cache
static uint8_t lastStableState[NUM_BUTTONS] = {0};
static uint8_t lastReadState[NUM_BUTTONS] = {0};
static unsigned long lastDebounceTime[NUM_BUTTONS] = {0};

void initButtons() {
  for (uint8_t i = 0; i < NUM_BUTTONS; i++) {
    pinMode(buttonPins[i], INPUT_PULLUP);
    lastReadState[i] = digitalRead(buttonPins[i]);
    lastStableState[i] = (lastReadState[i] == LOW) ? 1 : 0;
    buttonStates[i] = lastStableState[i];
    lastDebounceTime[i] = millis();
  }
}

void updateButtons() {
  unsigned long currentTime = millis();

  for (uint8_t i = 0; i < NUM_BUTTONS; i++) {
    uint8_t rawState = digitalRead(buttonPins[i]);
    uint8_t pressed = (rawState == LOW) ? 1 : 0;

    if (pressed != lastReadState[i]) {
      lastDebounceTime[i] = currentTime; // State changed → Remember time
      lastReadState[i] = pressed;
    }

    if ((currentTime - lastDebounceTime[i]) >= DEBOUNCE_DELAY) {
      if (pressed != lastStableState[i]) {
        lastStableState[i] = pressed;
        buttonStates[i] = pressed;
      }
    }
  }
}
