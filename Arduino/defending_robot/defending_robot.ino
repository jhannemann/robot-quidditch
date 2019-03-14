/*
Wiring Diagram for QTI Sensors:
Arduino          Sensor
D7               QTI4 - Far left
D6               QTI3 - Mid left
D5               QTI2 - Mid right
D4               QTI1 - Far right

Wiring Diagram for Servos:
Arduino          Servo
D13              Left servo
D12              Right servo
*/

#include <Servo.h>

// #define NDEBUG

const int BASE_SPEED = 150; // microseconds
const int TURN = 100; // microseconds
const int BASE_FREQ = 1500; // microseconds
const int MAX_DISTANCE = 150; //cm
const int QTI_CHARGE_TIME = 330; // microseconds
const int QTI_DISCHARGE_TIME = 330; // microseconds
const int STARTUP_DELAY = 1000; // milliseconds
const unsigned long DEBOUNCE_DELAY = 100; // milliseconds
const unsigned DISTANCE_ARRAY_SIZE = 16;

const int TRIG_PIN = 9;
const int ECHO_PIN = 10;
const int SWITCH_PIN = 11;
const int SERVO_L_PIN = 13;
const int SERVO_R_PIN = 12;

bool reverse = false;
bool switchState;
bool lastSwitchState = true;
unsigned long lastDebounceTime = 0;
// use a average for distance to act as a low-pass filter
int distances[DISTANCE_ARRAY_SIZE];
int currentDistance = 0;

Servo servoL;
Servo servoR;

void forward() {
  if(reverse) {
    servoL.writeMicroseconds(BASE_FREQ - BASE_SPEED);
    servoR.writeMicroseconds(BASE_FREQ + BASE_SPEED);
  }
  else {
    servoL.writeMicroseconds(BASE_FREQ + BASE_SPEED);
    servoR.writeMicroseconds(BASE_FREQ - BASE_SPEED);
  }
}

void stop() {
  servoL.writeMicroseconds(BASE_FREQ);
  servoR.writeMicroseconds(BASE_FREQ);
}

void turnLeft(int amount) {
  if(reverse) {
    servoL.writeMicroseconds(BASE_FREQ - BASE_SPEED);
    servoR.writeMicroseconds(BASE_FREQ + BASE_SPEED - amount*TURN);
  }
  else {
    servoL.writeMicroseconds(BASE_FREQ + BASE_SPEED - amount*TURN);
    servoR.writeMicroseconds(BASE_FREQ - BASE_SPEED);
  }
}

void turnRight(int amount) {
  if(reverse) {
    servoL.writeMicroseconds(BASE_FREQ - BASE_SPEED + amount*TURN);
    servoR.writeMicroseconds(BASE_FREQ + BASE_SPEED);
  }
  else {
    servoL.writeMicroseconds(BASE_FREQ + BASE_SPEED);
    servoR.writeMicroseconds(BASE_FREQ - BASE_SPEED + amount*TURN);
  }
}

 int sensorDistance() {
  digitalWrite(TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);
  // sound wave travel time in microseconds
  long duration = pulseIn(ECHO_PIN, HIGH);
  int distance = duration*0.034/2; // cm
  currentDistance = (currentDistance+1)%DISTANCE_ARRAY_SIZE;
  distances[currentDistance] = distance;
  int averageDistance = 0;
  for(int i=0; i<DISTANCE_ARRAY_SIZE; ++i) {
    averageDistance += distances[i];
  }
  return averageDistance/DISTANCE_ARRAY_SIZE;
  //return distance;
 }

// Perform these steps with the Arduino is first powered on
void setup()
{
#ifndef NDEBUG
  Serial.begin(9600);
 #endif
  for(int i=0; i<DISTANCE_ARRAY_SIZE; ++i) {
    distances[i] = 0;
  }
  servoL.attach(SERVO_L_PIN);
  servoR.attach(SERVO_R_PIN);
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT); 
  pinMode(SWITCH_PIN, INPUT_PULLUP);
  stop();
  delay(STARTUP_DELAY);
}

// This code repeats indefinitely
void loop()
{
  DDRD |= B11110000;                     // Set direction of Arduino pins D4-D7 as OUTPUT
  PORTD |= B11110000;                    // Set level of Arduino pins D4-D7 to HIGH
  delayMicroseconds(QTI_CHARGE_TIME);    // Charge capacitor in QTI module
  DDRD &= B00001111;                     // Set direction of pins D4-D7 as INPUT
  PORTD &= B00001111;                    // Set level of pins D4-D7 to LOW
  delayMicroseconds(QTI_DISCHARGE_TIME); // Discharge capacitor in QTI module
  uint16_t pins = PIND;                  // Get values of pins D0-D7
  pins >>= 4U;                           // Keep only pins D4-D7

#ifndef NDEBUG
  Serial.print("Pins: ");
  Serial.print((bool)(pins & B1000));
  Serial.print((bool)(pins & B0100));
  Serial.print((bool)(pins & B0010));
  Serial.print((bool)(pins & B0001));
#endif

  bool switchReading = digitalRead(SWITCH_PIN);
  if ((millis() - lastDebounceTime) > DEBOUNCE_DELAY) {
      // whatever the reading is at, it's been there for longer than the debounce
      // delay, so take it as the actual current state:
  
      // if the button state has changed:
      if (switchReading != switchState) {
        switchState = switchReading;
  
        if (switchState == LOW) {
          reverse = !reverse;
          stop();
        }
      }
  }

#ifndef NDEBUG
  Serial.print(", Switch: "); 
  Serial.print(switchReading);
  Serial.print(", Reverse: ");
  Serial.print(reverse);
#endif

  int distance = sensorDistance();

 #ifndef NDEBUG
  Serial.print(", Distance: ");
  Serial.println(distance);
 #endif

  if(distance > MAX_DISTANCE)
    stop();
  else
    forward();

//  switch(pins) {
//    case B0001:
//      if(reverse)
//        turnLeft(2);
//      else
//        turnRight(2);
//      break;
//    case B0011:
//    case B0100:
//    case B0111:
//      if(reverse)
//        turnLeft(1);
//      else
//        turnRight(1);
//      break;
//    case B0110:
//      forward();
//      break;
//    case B0010:
//    case B1100:
//    case B1110:
//      if(reverse)
//        turnRight(1);
//      else
//        turnLeft(1);
//      break;
//    case B1000:
//      if(reverse)
//        turnRight(2);
//      else
//        turnLeft(2);
//      break;
//    default:
//      stop();
//}
  
  delay(50);
}
