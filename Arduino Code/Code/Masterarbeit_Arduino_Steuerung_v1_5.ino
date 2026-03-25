#include "VibMotorControl.h"
#include "ServoControl.h"
#include "buttons.h"
#include "ledcontrol.h"
#include "ModuleController.h"


void setup() {
  Serial.begin(115200);
  
  setupVibMotors();
  servoControlBegin();
  initButtons();

  setupLEDControllers();
  initializeLEDColors();  // Set colors from the array

  delay(1000);
}



void loop() {
  // Updates the vibration motors without blocking the loop
  updateVibMotors();
  // Updates the servo positions without blocking the loop
  servoControlUpdate();
  // Updates the button array without blocking the loop
  updateButtons();
  // Updates the LED array without blocking the loop
  initializeLEDColors();  // Set colors from the array


  // Communication-Code
  if (Serial.available()) {
    String line = Serial.readStringUntil('\n');
    processLine(line);
  }

  //delay(10); // Optional; adjust for performance
}
