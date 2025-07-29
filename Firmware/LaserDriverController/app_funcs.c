#include "app_funcs.h"
#include "app_ios_and_regs.h"
#include "hwbp_core.h"

#define F_CPU 32000000
#include <util/delay.h>

#include "WS2812S.h"
#include "structs.h"

extern interval_t s_bnc_0, s_bnc_1, s_signal_a, s_signal_b;
extern countdown_t pulse_countdown;
extern ports_state_t state_on_or_off;
//ports_state_t _states_;
/************************************************************************/
/* Create pointers to functions                                         */
/************************************************************************/
extern AppRegs app_regs;
void start_signal(interval_t *signal, uint16_t t_on_ms, uint16_t n_pulses, uint16_t t_off_ms, uint16_t t_tail_ms);


void (*app_func_rd_pointer[])(void) = {
	&app_read_REG_SPAD_SWITCH,
	&app_read_REG_LASER_STATE,
	&app_read_REG_RESERVED0,
	&app_read_REG_RESERVED1,
	&app_read_REG_RESERVED2,
	&app_read_REG_RESERVED3,
	&app_read_REG_LASER_FREQUENCY_SELECT,
	&app_read_REG_LASER_INTENSITY,
	&app_read_REG_OUTPUT_SET,
	&app_read_REG_OUTPUT_CLEAR,
	&app_read_REG_OUTPUT_TOGGLE,
	&app_read_REG_OUTPUT_STATE,
	&app_read_REG_BNCS_STATE,
	&app_read_REG_SIGNAL_STATE,
	&app_read_REG_BNC0_ON,
	&app_read_REG_BNC0_OFF,
	&app_read_REG_BNC0_PULSES,
	&app_read_REG_BNC0_TAIL,
	&app_read_REG_BNC1_ON,
	&app_read_REG_BNC1_OFF,
	&app_read_REG_BNC1_PULSES,
	&app_read_REG_BNC1_TAIL,
	&app_read_REG_SIGNAL_A_ON,
	&app_read_REG_SIGNAL_A_OFF,
	&app_read_REG_SIGNAL_A_PULSES,
	&app_read_REG_SIGNAL_A_TAIL,
	&app_read_REG_SIGNAL_B_ON,
	&app_read_REG_SIGNAL_B_OFF,
	&app_read_REG_SIGNAL_B_PULSES,
	&app_read_REG_SIGNAL_B_TAIL,
	&app_read_REG_EVNT_ENABLE
};

bool (*app_func_wr_pointer[])(void*) = {
	&app_write_REG_SPAD_SWITCH,
	&app_write_REG_LASER_STATE,
	&app_write_REG_RESERVED0,
	&app_write_REG_RESERVED1,
	&app_write_REG_RESERVED2,
	&app_write_REG_RESERVED3,
	&app_write_REG_LASER_FREQUENCY_SELECT,
	&app_write_REG_LASER_INTENSITY,
	&app_write_REG_OUTPUT_SET,
	&app_write_REG_OUTPUT_CLEAR,
	&app_write_REG_OUTPUT_TOGGLE,
	&app_write_REG_OUTPUT_STATE,
	&app_write_REG_BNCS_STATE,
	&app_write_REG_SIGNAL_STATE,
	&app_write_REG_BNC0_ON,
	&app_write_REG_BNC0_OFF,
	&app_write_REG_BNC0_PULSES,
	&app_write_REG_BNC0_TAIL,
	&app_write_REG_BNC1_ON,
	&app_write_REG_BNC1_OFF,
	&app_write_REG_BNC1_PULSES,
	&app_write_REG_BNC1_TAIL,
	&app_write_REG_SIGNAL_A_ON,
	&app_write_REG_SIGNAL_A_OFF,
	&app_write_REG_SIGNAL_A_PULSES,
	&app_write_REG_SIGNAL_A_TAIL,
	&app_write_REG_SIGNAL_B_ON,
	&app_write_REG_SIGNAL_B_OFF,
	&app_write_REG_SIGNAL_B_PULSES,
	&app_write_REG_SIGNAL_B_TAIL,
	&app_write_REG_EVNT_ENABLE
};

/*#define start_BNC_SIG1_O do {set_BNC_SIG1_O; if (app_regs.REG_BNC_STATE & B_BNC0) pulse_countdown.bnc_0 = app_regs.REG_BNC0_ON + 1; } while(0)
#define start_BNC_SIG2_O do {set_BNC_SIG2_O; if (app_regs.REG_BNC_STATE & B_BNC1) pulse_countdown.bnc_1 = app_regs.REG_BNC1_ON + 1; } while(0)
#define start_SIGNAL_A_O do {set_SIGNAL_A_O; if (app_regs.REG_SIGNAL_STATE & B_SIGNAL_A) pulse_countdown.signal_a = app_regs.REG_SIGNAL_A_ON + 1; } while(0)
#define start_SIGNAL_B_O do {set_SIGNAL_B_O; if (app_regs.REG_SIGNAL_STATE & B_SIGNAL_B) pulse_countdown.signal_b = app_regs.REG_SIGNAL_B_ON + 1; } while(0)

#define off_start_SIGNAL_B_O do {clr_SIGNAL_B_O; if (app_regs.REG_SIGNAL_STATE & B_SIGNAL_B) pulse_countdown.signal_b_off = app_regs.REG_SIGNAL_B_OFF + 1; } while(0)
*/



/************************************************************************/
/* REG_SPAD_SWITCH                                                      */
/************************************************************************/
void app_read_REG_SPAD_SWITCH(void)
{
	//app_regs.REG_SPAD_SWITCH = 0;

}

bool app_write_REG_SPAD_SWITCH(void *a)
{
	uint8_t reg = *((uint8_t*)a);
	if(reg==0 || reg==1){
		spad_switch_events(reg);
	}

	return true;
}


/************************************************************************/
/* REG_LASER_STATE                                                      */
/************************************************************************/
void app_read_REG_LASER_STATE(void)
{
	//app_regs.REG_LASER_STATE = 0;

}

bool app_write_REG_LASER_STATE(void *a)
{
	//uint8_t reg = *((uint8_t*)a);

	//app_regs.REG_LASER_STATE = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED0                                                        */
/************************************************************************/
void app_read_REG_RESERVED0(void)
{
	//app_regs.REG_RESERVED0 = 0;

}

bool app_write_REG_RESERVED0(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_RESERVED0 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED1                                                        */
/************************************************************************/
void app_read_REG_RESERVED1(void)
{
	//app_regs.REG_RESERVED1 = 0;

}

bool app_write_REG_RESERVED1(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_RESERVED1 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED2                                                        */
/************************************************************************/
void app_read_REG_RESERVED2(void)
{
	//app_regs.REG_RESERVED2 = 0;

}

bool app_write_REG_RESERVED2(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED2 = reg;
	return true;
}


/************************************************************************/
/* REG_RESERVED3                                                        */
/************************************************************************/
void app_read_REG_RESERVED3(void)
{
	//app_regs.REG_RESERVED3 = 0;

}

bool app_write_REG_RESERVED3(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_RESERVED3 = reg;
	return true;
}


/************************************************************************/
/* REG_LASER_FREQUENCY_SELECT                                           */
/************************************************************************/
void app_read_REG_LASER_FREQUENCY_SELECT(void)
{
	//app_regs.REG_LASER_FREQUENCY_SELECT = 0;

}

bool app_write_REG_LASER_FREQUENCY_SELECT(void *a)
{
	uint8_t reg = *((uint8_t*)a);
	//1-F1, 2-F2, 4-F3, 7-CW
	if(reg==0 || reg==1 || reg==2 || reg==4 || reg==8){
		set_laser_freq(reg);
		//app_regs.REG_LASER_FREQUENCY_SELECT = reg;
	}


	return true;
}


/************************************************************************/
/* REG_LASER_INTENSITY                                                  */
/************************************************************************/
void app_read_REG_LASER_INTENSITY(void)
{
	//app_regs.REG_LASER_INTENSITY = 0;

}

bool app_write_REG_LASER_INTENSITY(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if( (reg >= 0) && (reg <= 255) ){
		app_regs.REG_LASER_INTENSITY = reg;
		update_digipot(app_regs.REG_LASER_INTENSITY, &SPID, &PORTD, 4, &PORTD); 
	}

	return true;
}


/************************************************************************/
/* REG_OUTPUT_SET                                                       */
/************************************************************************/
void app_read_REG_OUTPUT_SET(void)
{
	//app_regs.REG_OUTPUT_SET = 0;

}

bool app_write_REG_OUTPUT_SET(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_DOUT1) set_DO1;
	if (reg & B_DOUT2) set_DO2;

	app_regs.REG_OUTPUT_STATE |= reg;
	app_regs.REG_OUTPUT_SET = reg;

	return true;
}


/************************************************************************/
/* REG_OUTPUT_CLEAR                                                     */
/************************************************************************/
void app_read_REG_OUTPUT_CLEAR(void)
{
	//app_regs.REG_OUTPUT_CLEAR = 0;

}

bool app_write_REG_OUTPUT_CLEAR(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_DOUT1) clr_DO1;
	if (reg & B_DOUT2) clr_DO2;

	app_regs.REG_OUTPUT_STATE &= ~reg;
	app_regs.REG_OUTPUT_CLEAR = reg;

	return true;
}


/************************************************************************/
/* REG_OUTPUT_TOGGLE                                                    */
/************************************************************************/
void app_read_REG_OUTPUT_TOGGLE(void)
{
	//app_regs.REG_OUTPUT_TOGGLE = 0;

}

bool app_write_REG_OUTPUT_TOGGLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_DOUT1) { if (read_DO1) tgl_DO1; else set_DO1;}
	if (reg & B_DOUT2) { if (read_DO2) tgl_DO2; else set_DO2;}

	app_regs.REG_OUTPUT_STATE ^= reg;
	app_regs.REG_OUTPUT_TOGGLE = reg;

	return true;
}


/************************************************************************/
/* REG_OUTPUT_STATE                                                     */
/************************************************************************/
void app_read_REG_OUTPUT_STATE(void)
{
	//app_regs.REG_OUTPUT_STATE = 0;
	app_regs.REG_OUTPUT_STATE |= (read_DO1) ? B_DOUT1 : 0;
	app_regs.REG_OUTPUT_STATE |= (read_DO2) ? B_DOUT2 : 0;

}

bool app_write_REG_OUTPUT_STATE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	if (reg & B_DOUT1) set_DO1; else clr_DO1;
	if (reg & B_DOUT2) set_DO2; else clr_DO2;

	app_regs.REG_OUTPUT_STATE = reg;
	return true;
}




/************************************************************************/
/* REG_BNCS_STATE                                                       */
/************************************************************************/
void app_read_REG_BNCS_STATE(void)
{
	//app_regs.REG_BNCS_STATE = 0;

}

bool app_write_REG_BNCS_STATE(void *a)
{
	uint8_t reg = *((uint8_t*)a);


	if (reg & B_BNC0){
		start_signal(&s_bnc_0,  app_regs.REG_BNC0_ON,app_regs.REG_BNC0_PULSES, app_regs.REG_BNC0_OFF, app_regs.REG_BNC0_TAIL);
		pulse_countdown.tail_bnc0 = app_regs.REG_BNC0_TAIL + 1;			// discard first count (callback reasons)
		pulse_countdown.period_bnc0 =app_regs.REG_BNC0_ON + app_regs.REG_BNC0_OFF  ;			// discard first count (callback reasons)
		pulse_countdown.t_bnc0 = app_regs.REG_BNC0_ON ;				// discard first count (callback reasons)
		pulse_countdown.count_pulses_bnc0 = app_regs.REG_BNC0_PULSES + 1; // if 0 -> infinite pulses		
		
		if(pulse_countdown.tail_bnc0 == 1)
			set_BNC_SIG1_O;
			
	} 
	else clr_BNC_SIG1_O;
	
	
	if (reg & B_BNC1){
		start_signal(&s_bnc_1,  app_regs.REG_BNC1_ON,app_regs.REG_BNC1_PULSES, app_regs.REG_BNC1_OFF, app_regs.REG_BNC1_TAIL);
		pulse_countdown.tail_bnc1 = app_regs.REG_BNC1_TAIL + 1;			// discard first count (callback reasons)
		pulse_countdown.period_bnc1 =app_regs.REG_BNC1_ON + app_regs.REG_BNC1_OFF  ;			// discard first count (callback reasons)
		pulse_countdown.t_bnc1 = app_regs.REG_BNC1_ON ;				// discard first count (callback reasons)
		pulse_countdown.count_pulses_bnc1 = app_regs.REG_BNC1_PULSES + 1; // if 0 -> infinite pulses
		
		if(pulse_countdown.tail_bnc1 == 1)
			set_BNC_SIG2_O;
	}
	else clr_BNC_SIG2_O;



	app_regs.REG_BNCS_STATE = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_STATE                                                     */
/************************************************************************/
void app_read_REG_SIGNAL_STATE(void)
{
	//app_regs.REG_SIGNAL_STATE = 0;

}

bool app_write_REG_SIGNAL_STATE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_SIGNAL_STATE = reg;
	return true;
}


/************************************************************************/
/* REG_BNC0_ON                                                          */
/************************************************************************/
void app_read_REG_BNC0_ON(void)
{
	//app_regs.REG_BNC0_ON = 0;

}

bool app_write_REG_BNC0_ON(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC0_ON = reg;
	return true;
}


/************************************************************************/
/* REG_BNC0_OFF                                                         */
/************************************************************************/
void app_read_REG_BNC0_OFF(void)
{
	//app_regs.REG_BNC0_OFF = 0;

}

bool app_write_REG_BNC0_OFF(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC0_OFF = reg;
	return true;
}


/************************************************************************/
/* REG_BNC0_PULSES                                                      */
/************************************************************************/
void app_read_REG_BNC0_PULSES(void)
{
	//app_regs.REG_BNC0_PULSES = 0;

}

bool app_write_REG_BNC0_PULSES(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC0_PULSES = reg;
	return true;
}


/************************************************************************/
/* REG_BNC0_TAIL                                                        */
/************************************************************************/
void app_read_REG_BNC0_TAIL(void)
{
	//app_regs.REG_BNC0_TAIL = 0;

}

bool app_write_REG_BNC0_TAIL(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC0_TAIL = reg;
	return true;
}


/************************************************************************/
/* REG_BNC1_ON                                                          */
/************************************************************************/
void app_read_REG_BNC1_ON(void)
{
	//app_regs.REG_BNC1_ON = 0;

}

bool app_write_REG_BNC1_ON(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC1_ON = reg;
	return true;
}


/************************************************************************/
/* REG_BNC1_OFF                                                         */
/************************************************************************/
void app_read_REG_BNC1_OFF(void)
{
	//app_regs.REG_BNC1_OFF = 0;

}

bool app_write_REG_BNC1_OFF(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC1_OFF = reg;
	return true;
}


/************************************************************************/
/* REG_BNC1_PULSES                                                      */
/************************************************************************/
void app_read_REG_BNC1_PULSES(void)
{
	//app_regs.REG_BNC1_PULSES = 0;

}

bool app_write_REG_BNC1_PULSES(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC1_PULSES = reg;
	return true;
}


/************************************************************************/
/* REG_BNC1_TAIL                                                        */
/************************************************************************/
void app_read_REG_BNC1_TAIL(void)
{
	//app_regs.REG_BNC1_TAIL = 0;

}

bool app_write_REG_BNC1_TAIL(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_BNC1_TAIL = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_A_ON                                                      */
/************************************************************************/
void app_read_REG_SIGNAL_A_ON(void)
{
	//app_regs.REG_SIGNAL_A_ON = 0;

}

bool app_write_REG_SIGNAL_A_ON(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_A_ON = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_A_OFF                                                     */
/************************************************************************/
void app_read_REG_SIGNAL_A_OFF(void)
{
	//app_regs.REG_SIGNAL_A_OFF = 0;

}

bool app_write_REG_SIGNAL_A_OFF(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_A_OFF = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_A_PULSES                                                  */
/************************************************************************/
void app_read_REG_SIGNAL_A_PULSES(void)
{
	//app_regs.REG_SIGNAL_A_PULSES = 0;

}

bool app_write_REG_SIGNAL_A_PULSES(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_A_PULSES = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_A_TAIL                                                    */
/************************************************************************/
void app_read_REG_SIGNAL_A_TAIL(void)
{
	//app_regs.REG_SIGNAL_A_TAIL = 0;

}

bool app_write_REG_SIGNAL_A_TAIL(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_A_TAIL = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_B_ON                                                      */
/************************************************************************/
void app_read_REG_SIGNAL_B_ON(void)
{
	//app_regs.REG_SIGNAL_B_ON = 0;

}

bool app_write_REG_SIGNAL_B_ON(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_B_ON = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_B_OFF                                                     */
/************************************************************************/
void app_read_REG_SIGNAL_B_OFF(void)
{
	//app_regs.REG_SIGNAL_B_OFF = 0;

}

bool app_write_REG_SIGNAL_B_OFF(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_B_OFF = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_B_PULSES                                                  */
/************************************************************************/
void app_read_REG_SIGNAL_B_PULSES(void)
{
	//app_regs.REG_SIGNAL_B_PULSES = 0;

}

bool app_write_REG_SIGNAL_B_PULSES(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_B_PULSES = reg;
	return true;
}


/************************************************************************/
/* REG_SIGNAL_B_TAIL                                                    */
/************************************************************************/
void app_read_REG_SIGNAL_B_TAIL(void)
{
	//app_regs.REG_SIGNAL_B_TAIL = 0;

}

bool app_write_REG_SIGNAL_B_TAIL(void *a)
{
	uint16_t reg = *((uint16_t*)a);

	app_regs.REG_SIGNAL_B_TAIL = reg;
	return true;
}


/************************************************************************/
/* REG_EVNT_ENABLE                                                      */
/************************************************************************/
void app_read_REG_EVNT_ENABLE(void)
{
	//app_regs.REG_EVNT_ENABLE = 0;

}

bool app_write_REG_EVNT_ENABLE(void *a)
{
	uint8_t reg = *((uint8_t*)a);

	app_regs.REG_EVNT_ENABLE = reg;
	return true;
}


void start_signal(interval_t *signal, uint16_t t_on_ms, uint16_t n_pulses, uint16_t t_off_ms, uint16_t t_tail_ms){

	signal->on_ms = t_on_ms;
	signal->off_ms = t_off_ms;
	signal->pulses = n_pulses;
	signal->tail_ms = t_tail_ms;
	
	/*s_bnc_0.on_ms = t_on_ms;
	s_bnc_0.off_ms = t_off_ms;
	s_bnc_0.pulses = n_pulses;
	s_bnc_0.tail_ms = t_tail_ms;*/

}