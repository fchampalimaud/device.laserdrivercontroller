#ifndef _APP_FUNCTIONS_H_
#define _APP_FUNCTIONS_H_
#include <avr/io.h>

/************************************************************************/
/* Define if not defined                                                */
/************************************************************************/
#ifndef bool
#define bool uint8_t
#endif
#ifndef true
#define true 1
#endif
#ifndef false
#define false 0
#endif




/************************************************************************/
/* Prototypes                                                           */
/************************************************************************/
void app_read_REG_SPAD_SWITCH(void);
void app_read_REG_LASER_STATE(void);
void app_read_REG_RESERVED0(void);
void app_read_REG_RESERVED1(void);
void app_read_REG_RESERVED2(void);
void app_read_REG_RESERVED3(void);
void app_read_REG_LASER_FREQUENCY_SELECT(void);
void app_read_REG_LASER_INTENSITY(void);
void app_read_REG_OUTPUT_SET(void);
void app_read_REG_OUTPUT_CLEAR(void);
void app_read_REG_OUTPUT_TOGGLE(void);
void app_read_REG_OUTPUT_STATE(void);
void app_read_REG_BNCS_STATE(void);
void app_read_REG_SIGNAL_STATE(void);
void app_read_REG_BNC0_ON(void);
void app_read_REG_BNC0_OFF(void);
void app_read_REG_BNC0_PULSES(void);
void app_read_REG_BNC0_TAIL(void);
void app_read_REG_BNC1_ON(void);
void app_read_REG_BNC1_OFF(void);
void app_read_REG_BNC1_PULSES(void);
void app_read_REG_BNC1_TAIL(void);
void app_read_REG_SIGNAL_A_ON(void);
void app_read_REG_SIGNAL_A_OFF(void);
void app_read_REG_SIGNAL_A_PULSES(void);
void app_read_REG_SIGNAL_A_TAIL(void);
void app_read_REG_SIGNAL_B_ON(void);
void app_read_REG_SIGNAL_B_OFF(void);
void app_read_REG_SIGNAL_B_PULSES(void);
void app_read_REG_SIGNAL_B_TAIL(void);
void app_read_REG_EVNT_ENABLE(void);

bool app_write_REG_SPAD_SWITCH(void *a);
bool app_write_REG_LASER_STATE(void *a);
bool app_write_REG_RESERVED0(void *a);
bool app_write_REG_RESERVED1(void *a);
bool app_write_REG_RESERVED2(void *a);
bool app_write_REG_RESERVED3(void *a);
bool app_write_REG_LASER_FREQUENCY_SELECT(void *a);
bool app_write_REG_LASER_INTENSITY(void *a);
bool app_write_REG_OUTPUT_SET(void *a);
bool app_write_REG_OUTPUT_CLEAR(void *a);
bool app_write_REG_OUTPUT_TOGGLE(void *a);
bool app_write_REG_OUTPUT_STATE(void *a);
bool app_write_REG_BNCS_STATE(void *a);
bool app_write_REG_SIGNAL_STATE(void *a);
bool app_write_REG_BNC0_ON(void *a);
bool app_write_REG_BNC0_OFF(void *a);
bool app_write_REG_BNC0_PULSES(void *a);
bool app_write_REG_BNC0_TAIL(void *a);
bool app_write_REG_BNC1_ON(void *a);
bool app_write_REG_BNC1_OFF(void *a);
bool app_write_REG_BNC1_PULSES(void *a);
bool app_write_REG_BNC1_TAIL(void *a);
bool app_write_REG_SIGNAL_A_ON(void *a);
bool app_write_REG_SIGNAL_A_OFF(void *a);
bool app_write_REG_SIGNAL_A_PULSES(void *a);
bool app_write_REG_SIGNAL_A_TAIL(void *a);
bool app_write_REG_SIGNAL_B_ON(void *a);
bool app_write_REG_SIGNAL_B_OFF(void *a);
bool app_write_REG_SIGNAL_B_PULSES(void *a);
bool app_write_REG_SIGNAL_B_TAIL(void *a);
bool app_write_REG_EVNT_ENABLE(void *a);




void update_digipot(uint8_t data, SPI_t* spi, PORT_t* cs_port, uint8_t cs_pin, PORT_t* spi_port);
void spad_switch_events(uint8_t state);
void set_laser_freq(uint8_t value);

#endif /* _APP_FUNCTIONS_H_ */