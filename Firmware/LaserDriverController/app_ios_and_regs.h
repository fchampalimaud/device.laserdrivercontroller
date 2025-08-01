#ifndef _APP_IOS_AND_REGS_H_
#define _APP_IOS_AND_REGS_H_
#include "cpu.h"

void init_ios(void);
#define _SET_IO_ 0
#define _CLR_IO_ 1
#define _TGL_IO_ 2
/************************************************************************/
/* Definition of input pins                                             */
/************************************************************************/
// ON_OFF_KEY             Description: Flag Laser ON or OFF
// SWITCH_5V              Description: Switch input

#define read_ON_OFF_KEY read_io(PORTH, 0)       // ON_OFF_KEY
#define read_SWITCH_5V read_io(PORTE, 0)        // SWITCH_5V

/************************************************************************/
/* Definition of output pins                                            */
/************************************************************************/
// DO2                    Description: Output DO0
// DO1                    Description: Output DO1
// MCU_TO_RELAY           Description: Enable relay
// POT_CS                 Description: Potentiometer CS
// POT_SDI                Description: Potentiometer SDI
// POT_CLK                Description: Potentiometer CLK
// F1                     Description: Laser F1
// F2                     Description: Laser F2
// F3                     Description: Laser F3
// BNC_SIG1_O             Description: BNC signal 1
// BNC_SIG2_O             Description: BNC signal 2
// SIGNAL_A_O             Description: Optional signal A
// SIGNAL_B_O             Description: Optional signal B

/* DO2 */
#define set_DO2 set_io(PORTC, 2)
#define clr_DO2 clear_io(PORTC, 2)
#define tgl_DO2 toggle_io(PORTC, 2)
#define read_DO2 read_io(PORTC, 2)

/* DO1 */
#define set_DO1 set_io(PORTC, 0)
#define clr_DO1 clear_io(PORTC, 0)
#define tgl_DO1 toggle_io(PORTC, 0)
#define read_DO1 read_io(PORTC, 0)

/* MCU_TO_RELAY */
#define set_MCU_TO_RELAY set_io(PORTD, 1)
#define clr_MCU_TO_RELAY clear_io(PORTD, 1)
#define tgl_MCU_TO_RELAY toggle_io(PORTD, 1)
#define read_MCU_TO_RELAY read_io(PORTD, 1)

/* POT_CS */
#define set_POT_CS set_io(PORTD, 4)
#define clr_POT_CS clear_io(PORTD, 4)
#define tgl_POT_CS toggle_io(PORTD, 4)
#define read_POT_CS read_io(PORTD, 4)

/* POT_SDI */
#define set_POT_SDI set_io(PORTD, 5)
#define clr_POT_SDI clear_io(PORTD, 5)
#define tgl_POT_SDI toggle_io(PORTD, 5)
#define read_POT_SDI read_io(PORTD, 5)

/* POT_CLK */
#define set_POT_CLK set_io(PORTD, 7)
#define clr_POT_CLK clear_io(PORTD, 7)
#define tgl_POT_CLK toggle_io(PORTD, 7)
#define read_POT_CLK read_io(PORTD, 7)

/* F1 */
#define set_F1 set_io(PORTH, 3)
#define clr_F1 clear_io(PORTH, 3)
#define tgl_F1 toggle_io(PORTH, 3)
#define read_F1 read_io(PORTH, 3)

/* F2 */
#define set_F2 set_io(PORTH, 2)
#define clr_F2 clear_io(PORTH, 2)
#define tgl_F2 toggle_io(PORTH, 2)
#define read_F2 read_io(PORTH, 2)

/* F3 */
#define set_F3 set_io(PORTH, 1)
#define clr_F3 clear_io(PORTH, 1)
#define tgl_F3 toggle_io(PORTH, 1)
#define read_F3 read_io(PORTH, 1)

/* BNC_SIG1_O */
#define set_BNC_SIG1_O set_io(PORTJ, 0)
#define clr_BNC_SIG1_O clear_io(PORTJ, 0)
#define tgl_BNC_SIG1_O toggle_io(PORTJ, 0)
#define read_BNC_SIG1_O read_io(PORTJ, 0)

/* BNC_SIG2_O */
#define set_BNC_SIG2_O set_io(PORTJ, 2)
#define clr_BNC_SIG2_O clear_io(PORTJ, 2)
#define tgl_BNC_SIG2_O toggle_io(PORTJ, 2)
#define read_BNC_SIG2_O read_io(PORTJ, 2)

/* SIGNAL_A_O */
#define set_SIGNAL_A_O set_io(PORTJ, 4)
#define clr_SIGNAL_A_O clear_io(PORTJ, 4)
#define tgl_SIGNAL_A_O toggle_io(PORTJ, 4)
#define read_SIGNAL_A_O read_io(PORTJ, 4)

/* SIGNAL_B_O */
#define set_SIGNAL_B_O set_io(PORTJ, 7)
#define clr_SIGNAL_B_O clear_io(PORTJ, 7)
#define tgl_SIGNAL_B_O toggle_io(PORTJ, 7)
#define read_SIGNAL_B_O read_io(PORTJ, 7)


/************************************************************************/
/* Registers' structure                                                 */
/************************************************************************/
typedef struct
{
	uint8_t REG_SPAD_SWITCH;
	uint8_t REG_LASER_STATE;
	uint16_t REG_RESERVED0;
	uint16_t REG_RESERVED1;
	uint8_t REG_RESERVED2;
	uint8_t REG_RESERVED3;
	uint8_t REG_LASER_FREQUENCY_SELECT;
	uint8_t REG_LASER_INTENSITY;
	uint8_t REG_OUTPUT_SET;
	uint8_t REG_OUTPUT_CLEAR;
	uint8_t REG_OUTPUT_TOGGLE;
	uint8_t REG_OUTPUT_STATE;
	uint8_t REG_BNCS_STATE;
	uint8_t REG_SIGNAL_STATE;
	uint16_t REG_BNC0_ON;
	uint16_t REG_BNC0_OFF;
	uint16_t REG_BNC0_PULSES;
	uint16_t REG_BNC0_TAIL;
	uint16_t REG_BNC1_ON;
	uint16_t REG_BNC1_OFF;
	uint16_t REG_BNC1_PULSES;
	uint16_t REG_BNC1_TAIL;
	uint16_t REG_SIGNAL_A_ON;
	uint16_t REG_SIGNAL_A_OFF;
	uint16_t REG_SIGNAL_A_PULSES;
	uint16_t REG_SIGNAL_A_TAIL;
	uint16_t REG_SIGNAL_B_ON;
	uint16_t REG_SIGNAL_B_OFF;
	uint16_t REG_SIGNAL_B_PULSES;
	uint16_t REG_SIGNAL_B_TAIL;
	uint8_t REG_EVNT_ENABLE;
} AppRegs;

/************************************************************************/
/* Registers' address                                                   */
/************************************************************************/
/* Registers */
#define ADD_REG_SPAD_SWITCH                 32 // U8     Turns ON or OFF the relay to switch SPADs supply
#define ADD_REG_LASER_STATE                 33 // U8     State of the laser, if its ON or OFF
#define ADD_REG_RESERVED0                   34 // U16    Reserved for future use
#define ADD_REG_RESERVED1                   35 // U16    Reserved for future use
#define ADD_REG_RESERVED2                   36 // U8     Reserved for future use
#define ADD_REG_RESERVED3                   37 // U8     Reserved for future use
#define ADD_REG_LASER_FREQUENCY_SELECT      38 // U8     Set the laser frequency
#define ADD_REG_LASER_INTENSITY             39 // U8     Laser intensity value [0 : 255]
#define ADD_REG_OUTPUT_SET                  40 // U8     Set the correspondent output
#define ADD_REG_OUTPUT_CLEAR                41 // U8     Clear the correspondent output
#define ADD_REG_OUTPUT_TOGGLE               42 // U8     Toggle the correspondent output
#define ADD_REG_OUTPUT_STATE                43 // U8     Control the correspondent output
#define ADD_REG_BNCS_STATE                  44 // U8     Configures how BNCs will behave
#define ADD_REG_SIGNAL_STATE                45 // U8     Configures how signals will behave
#define ADD_REG_BNC0_ON                     46 // U16    Time ON of BNC1 (milliseconds) [1;65535]
#define ADD_REG_BNC0_OFF                    47 // U16    Time OFF of BNC1 (milliseconds) [1;65535]
#define ADD_REG_BNC0_PULSES                 48 // U16    Number of pulses (BNC1) [0;65535], 0-> infinite repeat
#define ADD_REG_BNC0_TAIL                   49 // U16    Wait time between pulses (milliseconds) (BNC1) [1;65535]
#define ADD_REG_BNC1_ON                     50 // U16    Time ON of BNC2 (milliseconds) [1;65535]
#define ADD_REG_BNC1_OFF                    51 // U16    Time OFF of BNC2 (milliseconds) [1;65535]
#define ADD_REG_BNC1_PULSES                 52 // U16    Number of pulses (BNC2) [0;65535], 0-> infinite repeat
#define ADD_REG_BNC1_TAIL                   53 // U16    Wait time between pulses (milliseconds) (BNC2) [1;65535]
#define ADD_REG_SIGNAL_A_ON                 54 // U16    Time ON of SIGNAL_A (milliseconds) [1;65535]
#define ADD_REG_SIGNAL_A_OFF                55 // U16    Time OFF of SIGNAL_A (milliseconds) [1;65535]
#define ADD_REG_SIGNAL_A_PULSES             56 // U16    Number of pulses (SIGNAL_A) [0;65535], 0-> infinite repeat
#define ADD_REG_SIGNAL_A_TAIL               57 // U16    Wait time between pulses (milliseconds) (SIGNAL_A) [1;65535]
#define ADD_REG_SIGNAL_B_ON                 58 // U16    Time ON of SIGNAL_B (milliseconds) [1;65535]
#define ADD_REG_SIGNAL_B_OFF                59 // U16    Time OFF of SIGNAL_B (milliseconds) [1;65535]
#define ADD_REG_SIGNAL_B_PULSES             60 // U16    Number of pulses (SIGNAL_B) [0;65535], 0-> infinite repeat
#define ADD_REG_SIGNAL_B_TAIL               61 // U16    Wait time between pulses (milliseconds) (SIGNAL_B) [1;65535]
#define ADD_REG_EVNT_ENABLE                 62 // U8     Enable the Events

/************************************************************************/
/* PWM Generator registers' memory limits                               */
/*                                                                      */
/* DON'T change the APP_REGS_ADD_MIN value !!!                          */
/* DON'T change these names !!!                                         */
/************************************************************************/
/* Memory limits */
#define APP_REGS_ADD_MIN                    0x20
#define APP_REGS_ADD_MAX                    0x3E
#define APP_NBYTES_OF_REG_BANK              49

/************************************************************************/
/* Registers' bits                                                      */
/************************************************************************/
#define B_F1                               (1<<0)       //
#define B_F2                               (1<<1)       //
#define B_F3                               (1<<2)       //
#define B_CW                               (1<<3)       //
#define B_DOUT1                            (1<<0)       // Digital output 1
#define B_DOUT2                            (1<<1)       // Digital output 2
#define B_BNC0                             (1<<0)       // BNC0  start/stop
#define B_BNC1                             (1<<1)       // BNC1  start/stop
#define B_SIGNAL_A                         (1<<0)       // SIGNAL_A  start/stop
#define B_SIGNAL_B                         (1<<1)       // SIGNAL_B start/stop
#define B_EVT_SPAD_SWITCH                  (1<<0)       // Event of register SPAD_SWITCH
#define B_EVT_LASER_STATE                  (1<<1)       // Event of register LASER_STATE

#endif /* _APP_REGS_H_ */