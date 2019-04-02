#include<Wire.h>
const int MPU=0x68;  // I2C address of the MPU-6050
int16_t AcX,AcY,AcZ,Tmp,GyX,GyY,GyZ;
//value returned is in interval [-32768, 32767] so for normalize multiply GyX and others for gyro_normalizer_factor
// float gyro_normalizer_factor = 1.0f / 32768.0f; 

void setup(){
  Wire.begin();
  Wire.beginTransmission(MPU);
  Wire.write(0x6B);  // PWR_MGMT_1 register
  Wire.write(0);     // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);
  Serial.begin(9600);
  pinMode(5, INPUT);//button JB
  pinMode(6, INPUT);//button A
  pinMode(7, INPUT);//button B
  pinMode(A0,INPUT);//X axis
  pinMode(A1,INPUT);//Y axis
  pinMode(LED_BUILTIN, OUTPUT);
  
  digitalWrite(5, LOW);
  digitalWrite(6, HIGH);
  digitalWrite(7, HIGH);
}

void loop(){

  Wire.beginTransmission(MPU);
  Wire.write(0x3B);  // starting with register 0x3B (ACCEL_XOUT_H)
  Wire.endTransmission(false);
  Wire.requestFrom(MPU,14,true);  // request a total of 14 registers
  AcX=Wire.read()<<8|Wire.read();  // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)     
  AcY=Wire.read()<<8|Wire.read();  // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)
  AcZ=Wire.read()<<8|Wire.read();  // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)
  Tmp=Wire.read()<<8|Wire.read();  // 0x41 (TEMP_OUT_H) & 0x42 (TEMP_OUT_L)
  GyX=Wire.read()<<8|Wire.read();  // 0x43 (GYRO_XOUT_H) & 0x44 (GYRO_XOUT_L)
  GyY=Wire.read()<<8|Wire.read();  // 0x45 (GYRO_YOUT_H) & 0x46 (GYRO_YOUT_L)
  GyZ=Wire.read()<<8|Wire.read();  // 0x47 (GYRO_ZOUT_H) & 0x48 (GYRO_ZOUT_L) 
  
  Serial.print(AcX); Serial.print(";"); Serial.print(AcY); Serial.print(";"); Serial.print(AcZ); Serial.print(";");
  Serial.print(GyX); Serial.print(";"); Serial.print(GyY); Serial.print(";"); Serial.print(GyZ); Serial.print(";");
  
     if (digitalRead(5) == HIGH)
    {
      
        Serial.print(4);
        Serial.print(";");
      //  Serial.flush();
      //  delay(25);

    }else{Serial.print(0);
        Serial.print(";");}
    if (digitalRead(6) == LOW)
    {
       
        Serial.print(1);
        Serial.print(";");

    }else{Serial.print(0);
        Serial.print(";");}

    if (digitalRead(7) == LOW)
    {
      
        Serial.print(2);
        Serial.print(";");
    } else{Serial.print(0);
        Serial.print(";");}

        //joystick value
    Serial.print(analogRead(A0)); Serial.print(";"); 
    Serial.print(analogRead(A1)); Serial.print(";");
    
  Serial.println("");
  Serial.flush();
  delay(25);

  
}
