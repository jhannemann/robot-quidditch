/*
Wiring Diagram for QTI Sensors:
Arduino          Sensor
D7               QTI3 - Left
D6               QTI2 - Mid
D5               QTI1 - Right

Wiring Diagram for Switches
D8               Front Switch
D9               Back Switch

Wiring Diagram for Ultrasound Sensor
D10              Trig
D11              Echo

Wiring Diagram for Servos:
Arduino          Servo
D13              Left servo
D12              Right servo
*/

#include <Servo.h>

//#define NDEBUG
//#define ENABLE_CALIBRATION
//#define ENABLE_QTI
//#define ENABLE_ULTRASND

const int BASE_SPEED = 130; // microseconds
const int LEFT_DIFFERENTIAL = 0.3*BASE_SPEED; // microseconds
const int REVERSE_BIAS = 0.28*BASE_SPEED;
const int TURN = 25; // microseconds
const int BASE_FREQ = 1500; // microseconds
const int MAX_DISTANCE = 150; //cm
const int QTI_CHARGE_TIME = 800; // microseconds
const int QTI_DISCHARGE_TIME = 800; // microseconds
const int STARTUP_DELAY = 2000; // milliseconds
const unsigned long DEBOUNCE_DELAY = 1; // milliseconds
const unsigned DISTANCE_ARRAY_SIZE = 16;

const int TRIG_PIN = 10;
const int ECHO_PIN = 11;
const int SERVO_L_PIN = 12;
const int SERVO_R_PIN = 13;

const int SWITCH_F_PIN = 8;
const int SWITCH_B_PIN = 9;

class Switch {
public:
  Switch(int pin) : m_pin(pin), m_switchState(HIGH),
                    m_lastSwitchState(HIGH), m_lastDebounceTime(0) {}
  bool state() {
    bool switchReading = digitalRead(m_pin);
    if(switchReading != m_lastSwitchState) {
      // reset the debouncing timer
      m_lastDebounceTime = millis();
    }
    if((millis() - m_lastDebounceTime) > DEBOUNCE_DELAY) {
      if(switchReading != m_switchState) {
        m_switchState = switchReading;
      }
    }
    m_lastSwitchState = switchReading;
    return m_switchState;
  }
private:
  int m_pin;
  bool m_switchState;
  bool m_lastSwitchState;
  unsigned long m_lastDebounceTime = 0;
};

Switch switchFront(SWITCH_F_PIN);
Switch switchBack(SWITCH_B_PIN);

bool reverse = false;

#ifdef ENABLE_ULTRASND
// use an average for distance to act as a low-pass filter
int distances[DISTANCE_ARRAY_SIZE];
int currentDistance = 0;
#endif

Servo servoL;
Servo servoR;

#ifdef ENABLE_CALIBRATION
void calibrationSequence() {
  stop();
  delay(3000);
  forward();
  delay(3000);
  stop();
  delay(1000);
  reverse=true;
  forward();
  delay(3000);
  stop();
  while(true);
}
#endif

void stop() {
  servoL.writeMicroseconds(BASE_FREQ);
  servoR.writeMicroseconds(BASE_FREQ);
}

void forward() {
  if(reverse) {
    servoL.writeMicroseconds(BASE_FREQ + BASE_SPEED + REVERSE_BIAS - LEFT_DIFFERENTIAL);
    servoR.writeMicroseconds(BASE_FREQ - BASE_SPEED - REVERSE_BIAS);
  }
  else {
    servoL.writeMicroseconds(BASE_FREQ - BASE_SPEED - LEFT_DIFFERENTIAL);
    servoR.writeMicroseconds(BASE_FREQ + BASE_SPEED);
  }
}

void turnLeft(int amount) {
  if(reverse) {
    servoL.writeMicroseconds(BASE_FREQ + BASE_SPEED + REVERSE_BIAS - LEFT_DIFFERENTIAL);
    servoR.writeMicroseconds(BASE_FREQ - BASE_SPEED - REVERSE_BIAS + amount*TURN);
  }
  else {
    servoL.writeMicroseconds(BASE_FREQ - BASE_SPEED - LEFT_DIFFERENTIAL + amount*TURN);
    servoR.writeMicroseconds(BASE_FREQ + BASE_SPEED);
  }
}

void turnRight(int amount) {
  if(reverse) {
    servoL.writeMicroseconds(BASE_FREQ + BASE_SPEED + REVERSE_BIAS - LEFT_DIFFERENTIAL - amount*TURN);
    servoR.writeMicroseconds(BASE_FREQ - BASE_SPEED - REVERSE_BIAS);
  }
  else {
    servoL.writeMicroseconds(BASE_FREQ - BASE_SPEED - LEFT_DIFFERENTIAL);
    servoR.writeMicroseconds(BASE_FREQ + BASE_SPEED - amount*TURN);
  }
}

#ifdef ENABLE_ULTRASND
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
 }
 #endif

void setup()
{
#ifndef NDEBUG
  Serial.begin(9600);
 #endif

  servoL.attach(SERVO_L_PIN);
  servoR.attach(SERVO_R_PIN);
  pinMode(SWITCH_F_PIN, INPUT_PULLUP);
  pinMode(SWITCH_B_PIN, INPUT_PULLUP);
  stop();
  delay(STARTUP_DELAY);

#ifdef ENABLE_ULTRASND
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT); 
  for(int i=0; i<DISTANCE_ARRAY_SIZE; ++i) {
    distances[i] = 0;
  }
#endif

#ifdef ENABLE_CALIBRATION
  calibrationSequence();
#endif
}

void loop()
{
#ifdef ENABLE_QTI
  DDRD |= B11110000;                     // Set direction of Arduino pins D4-D7 as OUTPUT
  PORTD |= B11110000;                    // Set level of Arduino pins D4-D7 to HIGH
  delayMicroseconds(QTI_CHARGE_TIME);    // Charge capacitor in QTI module
  DDRD &= B00001111;                     // Set direction of pins D4-D7 as INPUT
  PORTD &= B00001111;                    // Set level of pins D4-D7 to LOW
  delayMicroseconds(QTI_DISCHARGE_TIME); // Discharge capacitor in QTI module
  uint16_t pins = PIND;                  // Get values of pins D0-D7
  pins >>= 5U;                           // Keep only pins D5-D7

#ifndef NDEBUG
  Serial.print("Pins: ");
  Serial.print((bool)(pins & B0100));
  Serial.print((bool)(pins & B0010));
  Serial.print((bool)(pins & B0001));
#endif
#endif

  bool switchFrontReading = switchFront.state();
  bool switchBackReading = switchBack.state();
  if(switchFrontReading == LOW && switchBackReading == LOW) {
    stop();
  }
  else if(switchFrontReading == LOW) {
    stop();
    if(!reverse) {
      reverse = true;
      forward();
      delay(DEBOUNCE_DELAY);
    }
  }
  else if(switchBackReading == LOW) {
    stop();
    if(reverse){
      reverse = false;
      forward();
      delay(DEBOUNCE_DELAY);
    }
  }

#ifndef NDEBUG
  Serial.print(", Switch F: "); 
  Serial.print(switchFrontReading);
  Serial.print(", Switch B: "); 
  Serial.print(switchBackReading);
  Serial.print(", Reverse: ");
  Serial.println(reverse);
#endif

#ifdef ENABLE_ULTRASND
  int distance = sensorDistance();

 #ifndef NDEBUG
  Serial.print(", Distance: ");
  Serial.println(distance);
 #endif

  if(distance < MAX_DISTANCE) {
    stop();
    return;
  }
#endif //ENABLE_ULTRASND

  if(switchFrontReading == HIGH && switchBackReading == HIGH) {
#ifndef ENABLE_QTI
    forward();
#else
    switch(pins) {
      case B100:
        if(reverse)
          turnLeft(1);
        else
          turnRight(1);
        break;
      case B110:
        if(reverse)
          turnLeft(1);
        else
          turnRight(1);
        break;
      case B010:
        forward();
        break;
      case B001:
        if(reverse)
          turnRight(1);
        else
          turnLeft(1);
        break;
      case B011:
        if(reverse)
          turnRight(1);
        else
          turnLeft(1);
        break;
      case B111:
        stop();
        break;
      default:
        forward();
    }
#endif //ENABLE_QTI
  }
}
