#ifndef MODULE_CONTROLLER_H
#define MODULE_CONTROLLER_H

#include <Arduino.h>

#define NUM_MODULES 14
#define TOTAL_COMMANDS_PER_CYCLE 11

extern int receivedCommands;

void processLine(String line);
void sendButtonStates();

#endif
