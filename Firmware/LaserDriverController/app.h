#ifndef _APP_H_
#define _APP_H_
#include <avr/io.h>


/************************************************************************/
/* Enable the interrupts                                                */
/************************************************************************/
#define hwbp_app_enable_interrupts 	PMIC_CTRL = PMIC_CTRL | PMIC_RREN_bm | PMIC_LOLVLEN_bm | PMIC_MEDLVLEN_bm | PMIC_HILVLEN_bm; __asm volatile("sei");


/************************************************************************/
/* User prototypes                                                      */
/************************************************************************/
void init_calibration_values(void);


/************************************************************************/
/* Initialize the application                                           */
/************************************************************************/
void hwbp_app_initialize(void);
/*
void write_values_on_reg();
void request_temperature(uint8_t unit_id);
void request_gas(uint8_t unit_id);
void request_baud_rate(uint8_t unit_id);
void send_baud_rate(uint16_t baud_rate_value);
void request_version_n(uint8_t unit_id);
void request_serial_n(uint8_t unit_id);
void request_full_scale_flow(uint8_t unit_id);

void send_setpoint(float set_value);
void request_setpoint(uint8_t unit_id);*/
void clear_set_re_de();
#endif /* _APP_H_ */