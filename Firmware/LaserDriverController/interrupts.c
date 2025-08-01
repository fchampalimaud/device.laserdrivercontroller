#include "cpu.h"
#include "hwbp_core_types.h"
#include "app_ios_and_regs.h"
#include "app_funcs.h"
#include "hwbp_core.h"

/************************************************************************/
/* Declare application registers                                        */
/************************************************************************/
extern AppRegs app_regs;

/************************************************************************/
/* Interrupts from Timers                                               */
/************************************************************************/
// ISR(TCC0_OVF_vect, ISR_NAKED)

// ISR(TCD0_OVF_vect, ISR_NAKED)
// ISR(TCE0_OVF_vect, ISR_NAKED)
// ISR(TCF0_OVF_vect, ISR_NAKED)
//
// ISR(TCC0_CCA_vect, ISR_NAKED)
// ISR(TCD0_CCA_vect, ISR_NAKED)
// ISR(TCE0_CCA_vect, ISR_NAKED)
// ISR(TCF0_CCA_vect, ISR_NAKED)
//
// ISR(TCD1_OVF_vect, ISR_NAKED)
//
// ISR(TCD1_CCA_vect, ISR_NAKED)

/************************************************************************/
/* ON_OFF_KEY                                                           */
/************************************************************************/
ISR(PORTH_INT0_vect, ISR_NAKED)
{
	/*uint8_t reg_laser_state = app_regs.REG_LASER_STATE;
	if(read_ON_OFF_KEY){
		if(app_regs.REG_EVNT_ENABLE & B_EVT_LASER_STATE ){
			app_regs.REG_LASER_STATE = 1;
			if(reg_laser_state != app_regs.REG_LASER_STATE){
				core_func_send_event(ADD_REG_LASER_STATE, true);
			}
		}
	}
	else{
		if(app_regs.REG_EVNT_ENABLE & B_EVT_LASER_STATE ){
			app_regs.REG_LASER_STATE = 0;
			if(reg_laser_state != app_regs.REG_LASER_STATE){
				core_func_send_event(ADD_REG_LASER_STATE, true);
			}
		}
	}*/
	
	
	app_regs.REG_RESERVED2 = 2;

	reti();
}

/************************************************************************/
/* SWITCH_5V                                                            */
/************************************************************************/
//ONLY IF PHYSICAL SWITCH IS CONNECTED
ISR(PORTE_INT0_vect, ISR_NAKED)
{
	app_regs.REG_RESERVED1 = 2; 
	
	
	/*uint8_t reg_spad_switch = app_regs.REG_SPAD_SWITCH;

	if(read_SWITCH_5V){
		if(app_regs.REG_EVNT_ENABLE &  B_EVT_SPAD_SWITCH ){
			app_regs.REG_SPAD_SWITCH = 1;
			set_MCU_TO_RELAY;
			if(reg_spad_switch  != app_regs.REG_SPAD_SWITCH){
				core_func_send_event(ADD_REG_SPAD_SWITCH, true);
			}
		}
	}
	else{
		if(app_regs.REG_EVNT_ENABLE & B_EVT_SPAD_SWITCH ){
			app_regs.REG_SPAD_SWITCH = 0;
			clr_MCU_TO_RELAY;
			if(reg_spad_switch != app_regs.REG_SPAD_SWITCH){
				core_func_send_event(ADD_REG_SPAD_SWITCH, true);
			}
		}
	}*/

	reti();
}

