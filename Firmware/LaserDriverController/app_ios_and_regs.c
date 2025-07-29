#include <avr/io.h>
#include "hwbp_core_types.h"
#include "app_ios_and_regs.h"

/************************************************************************/
/* Configure and initialize IOs                                         */
/************************************************************************/
void init_ios(void)
{	/* Configure input pins */
	//io_pin2in(&PORTH, 0, PULL_IO_UP , SENSE_IO_EDGES_BOTH);                         // ON_OFF_KEY
	io_pin2in(&PORTE, 0, PULL_IO_DOWN, SENSE_IO_EDGES_BOTH);             // SWITCH_5V
	io_pin2in(&PORTA, 0, PULL_IO_DOWN , SENSE_IO_EDGES_BOTH);                         // ON_OFF_KEY

	/* Configure input interrupts */
	//io_set_int(&PORTH, INT_LEVEL_LOW, 0, (1<<0), false);                 // ON_OFF_KEY
	io_set_int(&PORTE, INT_LEVEL_LOW, 0, (1<<0), false);                 // SWITCH_5V
	io_set_int(&PORTA, INT_LEVEL_LOW, 0, (1<<0), false);                 // ON_OFF_KEY


	/* Configure output pins */
	io_pin2out(&PORTC, 2, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // DO2
	io_pin2out(&PORTC, 0, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // DO1
	io_pin2out(&PORTD, 1, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // MCU_TO_RELAY
	io_pin2out(&PORTD, 4, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // POT_CS
	io_pin2out(&PORTD, 5, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // POT_SDI
	io_pin2out(&PORTD, 7, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // POT_CLK
	io_pin2out(&PORTH, 3, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // F1
	io_pin2out(&PORTH, 2, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // F2
	io_pin2out(&PORTH, 1, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // F3
	io_pin2out(&PORTJ, 0, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // BNC_SIG1_O
	io_pin2out(&PORTJ, 2, OUT_IO_DIGITAL, IN_EN_IO_DIS);                 // BNC_SIG2_O
	io_pin2out(&PORTJ, 4, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // SIGNAL_A_O
	io_pin2out(&PORTJ, 7, OUT_IO_DIGITAL, IN_EN_IO_EN);                  // SIGNAL_B_O

	/* Initialize output pins */
	clr_DO2;
	clr_DO1;
	clr_MCU_TO_RELAY;
	clr_POT_CS;
	clr_POT_SDI;
	clr_POT_CLK;
	clr_F1;
	clr_F2;
	clr_F3;
	clr_BNC_SIG1_O;
	clr_BNC_SIG2_O;
	clr_SIGNAL_A_O;
	clr_SIGNAL_B_O;
}

/************************************************************************/
/* Registers' stuff                                                     */
/************************************************************************/
AppRegs app_regs;

uint8_t app_regs_type[] = {
	TYPE_U8,
	TYPE_U8,
	TYPE_U16,
	TYPE_U16,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U8,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U16,
	TYPE_U8
};

uint16_t app_regs_n_elements[] = {
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1,
	1
};

uint8_t *app_regs_pointer[] = {
	(uint8_t*)(&app_regs.REG_SPAD_SWITCH),
	(uint8_t*)(&app_regs.REG_LASER_STATE),
	(uint8_t*)(&app_regs.REG_RESERVED0),
	(uint8_t*)(&app_regs.REG_RESERVED1),
	(uint8_t*)(&app_regs.REG_RESERVED2),
	(uint8_t*)(&app_regs.REG_RESERVED3),
	(uint8_t*)(&app_regs.REG_LASER_FREQUENCY_SELECT),
	(uint8_t*)(&app_regs.REG_LASER_INTENSITY),
	(uint8_t*)(&app_regs.REG_OUTPUT_SET),
	(uint8_t*)(&app_regs.REG_OUTPUT_CLEAR),
	(uint8_t*)(&app_regs.REG_OUTPUT_TOGGLE),
	(uint8_t*)(&app_regs.REG_OUTPUT_STATE),
	(uint8_t*)(&app_regs.REG_BNCS_STATE),
	(uint8_t*)(&app_regs.REG_SIGNAL_STATE),
	(uint8_t*)(&app_regs.REG_BNC0_ON),
	(uint8_t*)(&app_regs.REG_BNC0_OFF),
	(uint8_t*)(&app_regs.REG_BNC0_PULSES),
	(uint8_t*)(&app_regs.REG_BNC0_TAIL),
	(uint8_t*)(&app_regs.REG_BNC1_ON),
	(uint8_t*)(&app_regs.REG_BNC1_OFF),
	(uint8_t*)(&app_regs.REG_BNC1_PULSES),
	(uint8_t*)(&app_regs.REG_BNC1_TAIL),
	(uint8_t*)(&app_regs.REG_SIGNAL_A_ON),
	(uint8_t*)(&app_regs.REG_SIGNAL_A_OFF),
	(uint8_t*)(&app_regs.REG_SIGNAL_A_PULSES),
	(uint8_t*)(&app_regs.REG_SIGNAL_A_TAIL),
	(uint8_t*)(&app_regs.REG_SIGNAL_B_ON),
	(uint8_t*)(&app_regs.REG_SIGNAL_B_OFF),
	(uint8_t*)(&app_regs.REG_SIGNAL_B_PULSES),
	(uint8_t*)(&app_regs.REG_SIGNAL_B_TAIL),
	(uint8_t*)(&app_regs.REG_EVNT_ENABLE)
};