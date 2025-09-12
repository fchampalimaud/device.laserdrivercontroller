#include "hwbp_core.h"
#include "hwbp_core_regs.h"
#include "hwbp_core_types.h"

#include "app.h"
#include "app_funcs.h"
#include "app_ios_and_regs.h"
#include "structs.h"

#define F_CPU 32000000 //need to be defined for delay.h
#include <util/delay.h>
/************************************************************************/
/* Declare application registers                                        */
/************************************************************************/
extern AppRegs app_regs;
extern uint8_t app_regs_type[];
extern uint16_t app_regs_n_elements[];
extern uint8_t *app_regs_pointer[];
extern void (*app_func_rd_pointer[])(void);
extern bool (*app_func_wr_pointer[])(void*);
extern ports_state_t state_on_or_off;
countdown_t pulse_countdown;
interval_t s_bnc_0, s_bnc_1, s_signal_a, s_signal_b;

#define _1_CLOCK_CYCLES asm ( "nop \n")
#define _2_CLOCK_CYCLES _1_CLOCK_CYCLES; _1_CLOCK_CYCLES
#define _4_CLOCK_CYCLES _2_CLOCK_CYCLES; _2_CLOCK_CYCLES
#define _8_CLOCK_CYCLES _4_CLOCK_CYCLES; _4_CLOCK_CYCLES


/************************************************************************/
/* Initialize app                                                       */
/************************************************************************/
static const uint8_t default_device_name[] = "LaserDriverController";

void hwbp_app_initialize(void)
{
    /* Define versions */
    uint8_t hwH = 1;
    uint8_t hwL = 0;
    uint8_t fwH = 1;
    uint8_t fwL = 0;
    uint8_t ass = 0;
    
   	/* Start core */
    core_func_start_core(
        1298,
        hwH, hwL,
        fwH, fwL,
        ass,
        (uint8_t*)(&app_regs),
        APP_NBYTES_OF_REG_BANK,
        APP_REGS_ADD_MAX - APP_REGS_ADD_MIN + 1,
        default_device_name,
		false,	// The device is _not_ able to repeat the harp timestamp clock
		false,	// The device is _not_ able to generate the harp timestamp clock
		0		// Default timestamp offset
    );
}

/************************************************************************/
/* Handle if a catastrophic error occur                                 */
/************************************************************************/
void core_callback_catastrophic_error_detected(void)
{
	
}

/************************************************************************/
/* User functions                                                       */
/************************************************************************/
/* Add your functions here or load external functions if needed */

void update_digipot(uint8_t data, SPI_t* spi, PORT_t* cs_port, uint8_t cs_pin, PORT_t* spi_port){

	spi->CTRL = 0;

	clear_io((*(PORT_t *)cs_port), cs_pin); //enable digipot
    _8_CLOCK_CYCLES;
	//Select channel 2 (01)
	clear_io((*(PORT_t *)spi_port), 5); //B9 =0 MOSI
	set_io((*(PORT_t *)spi_port), 5); //B8 =1 MOSI

	set_io((*(PORT_t *)spi_port), 7); 
    _8_CLOCK_CYCLES;        
    clear_io((*(PORT_t *)spi_port), 7);
    _8_CLOCK_CYCLES;

    for (uint8_t i = 0; i < 8; i++)
    {
         if ((data<<i) & 0x80)
            set_io((*(PORT_t *)spi_port), 5);
         else
            clear_io((*(PORT_t *)spi_port), 5);
         
         set_io((*(PORT_t *)spi_port), 7);
         _8_CLOCK_CYCLES;
         
         clear_io((*(PORT_t *)spi_port), 7);
         _8_CLOCK_CYCLES;
	}
	set_io((*(PORT_t *)cs_port), cs_pin);

}

//Update and send event of digital SPAD_SWITCH register
void spad_switch_events(uint8_t state){
	uint8_t reg_spad_switch = app_regs.REG_SPAD_SWITCH;
	if(state==1){
		if(app_regs.REG_EVNT_ENABLE &  B_EVT_SPAD_SWITCH ){
			app_regs.REG_SPAD_SWITCH = 1;
			set_MCU_TO_RELAY;
			if(reg_spad_switch != app_regs.REG_SPAD_SWITCH){
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
	}
}


void set_laser_freq(uint8_t value){
	if(value == 0){
		clr_F1;
		clr_F2;
		clr_F3;
		app_regs.REG_LASER_FREQUENCY_SELECT = 0;
	}
	if(value & B_F1){
		set_F1;
		clr_F2;
		clr_F3;
		app_regs.REG_LASER_FREQUENCY_SELECT = B_F1;
	}
	if(value & B_F2){
		clr_F1;
		set_F2;
		clr_F3;
		app_regs.REG_LASER_FREQUENCY_SELECT = B_F2;
	}
	if(value & B_F3){
		clr_F1;
		clr_F2;
		set_F3;
		app_regs.REG_LASER_FREQUENCY_SELECT = B_F3;
	}
	if(value & B_CW){
		set_F1;
		set_F2;
		set_F3;
		app_regs.REG_LASER_FREQUENCY_SELECT = B_CW;
	}

}











/************************************************************************/
/* Initialization Callbacks                                             */
/************************************************************************/
void core_callback_define_clock_default(void) {}

void core_callback_initialize_hardware(void)
{
	/* Initialize IOs */
	/* Don't delete this function!!! */
	init_ios();
		
	//uart1_init(12, 2, false);
	//uart1_enable();

	/* Initialize SPI with 4MHz */
   	SPID_CTRL = SPI_MASTER_bm | SPI_ENABLE_bm | SPI_MODE_0_gc | SPI_CLK2X_bm | SPI_PRESCALER_DIV16_gc;
	
	/*// Reset ADC 
	_delay_ms(100);
	set_RESET;
	_delay_ms(1);
	clr_RESET;*/
	_delay_ms(10);
	/* Initialize hardware */
	update_digipot(0, &SPID, &PORTD, 4, &PORTD);
}
void core_callback_1st_config_hw_after_boot(void)
{
	/* Initialize IOs */
	/* Don't delete this function!!! */
	init_ios();
	
	/* Initialize hardware */
	/* Initialize SPI with 4MHz */
	//SPIE_CTRL = SPI_MASTER_bm | SPI_ENABLE_bm | SPI_MODE_0_gc | SPI_CLK2X_bm | SPI_PRESCALER_DIV16_gc;
   	SPID_CTRL = SPI_MASTER_bm | SPI_ENABLE_bm | SPI_MODE_0_gc | SPI_CLK2X_bm | SPI_PRESCALER_DIV16_gc;
	
	/*// Reset ADC 
	_delay_ms(100);
	set_RESET;
	_delay_ms(1);
	clr_RESET;*/
	_delay_ms(10);
	update_digipot(0, &SPID, &PORTD, 4, &PORTD);
}

void core_callback_reset_registers(void)
{
	/* Initialize registers */

	app_regs.REG_SPAD_SWITCH = read_SWITCH_5V;
	app_regs.REG_LASER_STATE = read_ON_OFF_KEY;       
	app_regs.REG_RESERVED0 = 0;
	app_regs.REG_RESERVED1 = 0;            
	app_regs.REG_RESERVED2 = 0;
	app_regs.REG_RESERVED3 = 0;
	app_regs.REG_LASER_FREQUENCY_SELECT = 0;
	app_regs.REG_LASER_INTENSITY = 0;
	app_regs.REG_OUTPUT_SET = 0;
	app_regs.REG_OUTPUT_CLEAR = 0;
	app_regs.REG_OUTPUT_TOGGLE = 0;
	app_regs.REG_OUTPUT_STATE = 0;
	app_regs.REG_BNCS_STATE = 0;
	app_regs.REG_SIGNAL_STATE = 0;
	app_regs.REG_BNC0_ON = 0;
	app_regs.REG_BNC0_OFF = 0;
	app_regs.REG_BNC0_PULSES = 0;
	app_regs.REG_BNC0_TAIL = 0;
	app_regs.REG_BNC1_ON = 0;
	app_regs.REG_BNC1_OFF = 0;
	app_regs.REG_BNC1_PULSES = 0;
	app_regs.REG_BNC1_TAIL = 0;
	app_regs.REG_SIGNAL_A_ON = 0;
	app_regs.REG_SIGNAL_A_OFF = 0;
	app_regs.REG_SIGNAL_A_PULSES = 0;
	app_regs.REG_SIGNAL_A_TAIL = 0;
	app_regs.REG_SIGNAL_B_ON = 0;
	app_regs.REG_SIGNAL_B_OFF = 0;
	app_regs.REG_SIGNAL_B_PULSES = 0;
	app_regs.REG_SIGNAL_B_TAIL = 0;
	
	app_regs.REG_EVNT_ENABLE = B_EVT_SPAD_SWITCH | B_EVT_LASER_STATE; //enable events
	
}

void core_callback_registers_were_reinitialized(void)
{
	/* Update registers if needed */
	
}

/************************************************************************/
/* Callbacks: Visualization                                             */
/************************************************************************/
void core_callback_visualen_to_on(void)
{
	/* Update visual indicators */
	
}

void core_callback_visualen_to_off(void)
{
	/* Clear all the enabled indicators */
	
}

/************************************************************************/
/* Callbacks: Change on the operation mode                              */
/************************************************************************/
void core_callback_device_to_standby(void) {
	
	app_regs.REG_BNCS_STATE = 0;
	app_write_REG_BNCS_STATE(&app_regs.REG_BNCS_STATE);
	
	app_regs.REG_SIGNAL_STATE = 0;
	app_write_REG_SIGNAL_STATE(&app_regs.REG_SIGNAL_STATE);
	
	app_regs.REG_LASER_INTENSITY = 0;
	app_write_REG_LASER_INTENSITY(&app_regs.REG_LASER_INTENSITY);
	
	app_write_REG_SPAD_SWITCH(0);	
	
	
}
void core_callback_device_to_active(void) {}
void core_callback_device_to_enchanced_active(void) {}
void core_callback_device_to_speed(void) {}

/************************************************************************/
/* Callbacks: 1 ms timer                                                */
/************************************************************************/
void core_callback_t_before_exec(void) {}
void core_callback_t_after_exec(void) {}
void core_callback_t_new_second(void) {}
void core_callback_t_500us(void) {

	//----------------------------BNC SIGNAL 1-----------------------------
	//counts delay time before start signal
	if (app_regs.REG_BNCS_STATE & B_BNC0){
	if ((pulse_countdown.tail_bnc0)  > 0 && (app_regs.REG_BNC0_ON != 0)){	
			if (--pulse_countdown.tail_bnc0 == 0){
				set_BNC_SIG1_O;
			}
	}
	else{	
		//BNC 0 signal
		if (pulse_countdown.t_bnc0  > 0){	
			//--pulse_countdown.tail_bnc0;
			--pulse_countdown.period_bnc0;
			if (--pulse_countdown.t_bnc0 == 0){
				//if(pulse_countdown.tail_bnc0 == 0){ //ends pulse, goes to ON part of signal or stops the signal
				if(pulse_countdown.period_bnc0 == 0){ //ends pulse, goes to ON part of signal or stops the signal
					if( --pulse_countdown.count_pulses_bnc0 > 1  ){
					
						//pulse_countdown.tail_bnc0 = s_bnc_0.tail_ms ; //set the tail value
						pulse_countdown.period_bnc0 = s_bnc_0.on_ms + s_bnc_0.off_ms   ; //set the period value
						pulse_countdown.t_bnc0 = s_bnc_0.on_ms ;			
						set_BNC_SIG1_O;	
					}
					else if (pulse_countdown.count_pulses_bnc0 == 1){ //end
						clr_BNC_SIG1_O;
						app_regs.REG_BNCS_STATE &= ~B_BNC0; //stops signal
					}
					else{ //infinite pulses
						//pulse_countdown.tail_bnc0 = s_bnc_0.tail_ms; //set the tail value
						pulse_countdown.period_bnc0 =s_bnc_0.on_ms + s_bnc_0.off_ms; //set the period value
						pulse_countdown.t_bnc0 = s_bnc_0.on_ms ;
						pulse_countdown.count_pulses_bnc0 = 1;
						set_BNC_SIG1_O;	
					}				
				}
				else{ //goes to off part of signal
					pulse_countdown.t_bnc0 =  s_bnc_0.off_ms;
					clr_BNC_SIG1_O;
				}
			}		
		}
	}
	}

	//----------------------------BNC SIGNAL 2-----------------------------
	//counts delay time before start signal
	if (app_regs.REG_BNCS_STATE & B_BNC1){
	if ((pulse_countdown.tail_bnc1  > 0) && (app_regs.REG_BNC1_ON != 0)){
		if (--pulse_countdown.tail_bnc1 == 0 ){
			set_BNC_SIG2_O;
		}
	}
	else{
		//BNC 1 signal
		if (pulse_countdown.t_bnc1  > 0){
			//--pulse_countdown.tail_bnc1;
			--pulse_countdown.period_bnc1;
			if (--pulse_countdown.t_bnc1 == 0){
				//if(pulse_countdown.tail_bnc1 == 0){ //ends pulse, goes to ON part of signal or stops the signal
				if(pulse_countdown.period_bnc1 == 0){ //ends pulse, goes to ON part of signal or stops the signal
					if( --pulse_countdown.count_pulses_bnc1 > 1  ){
					
						//pulse_countdown.tail_bnc1 = s_bnc_1.tail_ms ; //set the tail value
						pulse_countdown.period_bnc1 = s_bnc_1.on_ms + s_bnc_1.off_ms   ; //set the period value
						pulse_countdown.t_bnc1 = s_bnc_1.on_ms ;
						set_BNC_SIG2_O;
					}
					else if (pulse_countdown.count_pulses_bnc1 == 1){ //end
						clr_BNC_SIG2_O;
						app_regs.REG_BNCS_STATE &= ~B_BNC1; //stops signal
					}
					else{ //infinite pulses
						//pulse_countdown.tail_bnc1 = s_bnc_1.tail_ms; //set the tail value
						pulse_countdown.period_bnc1 = s_bnc_1.on_ms + s_bnc_1.off_ms; //set the period value
						pulse_countdown.t_bnc1 = s_bnc_1.on_ms ;
						pulse_countdown.count_pulses_bnc1 = 1;
						set_BNC_SIG2_O;
					}
				}
				else{ //goes to off part of signal
					pulse_countdown.t_bnc1 =  s_bnc_1.off_ms;
					clr_BNC_SIG2_O;
				}
			}
		}
	}
	}

	//----------------------------SIGNAL A-----------------------------
	//counts delay time before start signal
	if (app_regs.REG_SIGNAL_STATE & B_SIGNAL_A){
	if ((pulse_countdown.tail_signal_a  > 0) && (app_regs.REG_SIGNAL_A_ON != 0)){
		if (--pulse_countdown.tail_signal_a == 0 ){
			set_SIGNAL_A_O;
		}
	}
	else{
		// signalA
		if (pulse_countdown.t_signal_a  > 0){
			//--pulse_countdown.tail_bnc1;
			--pulse_countdown.period_signal_a;
			if (--pulse_countdown.t_signal_a == 0){
				//if(pulse_countdown.tail_bnc1 == 0){ //ends pulse, goes to ON part of signal or stops the signal
				if(pulse_countdown.period_signal_a == 0){ //ends pulse, goes to ON part of signal or stops the signal
					if( --pulse_countdown.count_pulses_signal_a > 1  ){
						
						//pulse_countdown.tail_bnc1 = s_bnc_1.tail_ms ; //set the tail value
						pulse_countdown.period_signal_a = s_signal_a.on_ms + s_signal_a.off_ms   ; //set the period value
						pulse_countdown.t_signal_a = s_signal_a.on_ms ;
						set_SIGNAL_A_O;
					}
					else if (pulse_countdown.count_pulses_signal_a == 1){ //end
						clr_SIGNAL_A_O;
						app_regs.REG_SIGNAL_STATE &= ~B_SIGNAL_A; //stops signal
					}
					else{ //infinite pulses
						//pulse_countdown.tail_bnc1 = s_bnc_1.tail_ms; //set the tail value
						pulse_countdown.period_signal_a = s_signal_a.on_ms + s_signal_a.off_ms; //set the period value
						pulse_countdown.t_signal_a = s_signal_a.on_ms ;
						pulse_countdown.count_pulses_signal_a = 1;
						set_SIGNAL_A_O;
					}
				}
				else{ //goes to off part of signal
					pulse_countdown.t_signal_a =  s_signal_a.off_ms;
					clr_SIGNAL_A_O;
				}
			}
		}
	}
	
	}
	//----------------------------SIGNAL B-----------------------------
	//counts delay time before start signal
	if (app_regs.REG_SIGNAL_STATE & B_SIGNAL_B){
	if ((pulse_countdown.tail_signal_b  > 0) && (app_regs.REG_SIGNAL_B_ON != 0)){
		if (--pulse_countdown.tail_signal_b == 0 ){
			set_SIGNAL_B_O;
		}
	}
	else{
		// signalA
		if (pulse_countdown.t_signal_b  > 0){
			//--pulse_countdown.tail_bnc1;
			--pulse_countdown.period_signal_b;
			if (--pulse_countdown.t_signal_b == 0){
				//if(pulse_countdown.tail_bnc1 == 0){ //ends pulse, goes to ON part of signal or stops the signal
				if(pulse_countdown.period_signal_b == 0){ //ends pulse, goes to ON part of signal or stops the signal
					if( --pulse_countdown.count_pulses_signal_b > 1  ){
						
						//pulse_countdown.tail_bnc1 = s_bnc_1.tail_ms ; //set the tail value
						pulse_countdown.period_signal_b = s_signal_b.on_ms + s_signal_b.off_ms   ; //set the period value
						pulse_countdown.t_signal_b = s_signal_b.on_ms ;
						set_SIGNAL_B_O;
					}
					else if (pulse_countdown.count_pulses_signal_b == 1){ //end
						clr_SIGNAL_B_O;
						app_regs.REG_SIGNAL_STATE &= ~B_SIGNAL_B; //stops signal
					}
					else{ //infinite pulses
						//pulse_countdown.tail_bnc1 = s_bnc_1.tail_ms; //set the tail value
						pulse_countdown.period_signal_b = s_signal_b.on_ms + s_signal_b.off_ms; //set the period value
						pulse_countdown.t_signal_b = s_signal_b.on_ms ;
						pulse_countdown.count_pulses_signal_b = 1;
						set_SIGNAL_B_O;
					}
				}
				else{ //goes to off part of signal
					pulse_countdown.t_signal_b =  s_signal_b.off_ms;
					clr_SIGNAL_B_O;
				}
			}
		}
	}
	}
			
}
void core_callback_t_1ms(void) {
	
	//spad switch event from interrupt
	
	if (app_regs.REG_RESERVED1 == 2){
		uint8_t reg_spad_switch = app_regs.REG_SPAD_SWITCH;
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
		}
		app_regs.REG_RESERVED1 = 0;
		
	}
	
	//key switch event from interrupt
	if (app_regs.REG_RESERVED2 == 2){
		uint8_t reg_laser_state = app_regs.REG_LASER_STATE;
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
		}
		app_regs.REG_RESERVED2 =0;
	}
	
}



/************************************************************************/
/* Callbacks: clock control                                              */
/************************************************************************/
void core_callback_clock_to_repeater(void) {}
void core_callback_clock_to_generator(void) {}
void core_callback_clock_to_unlock(void) {}
void core_callback_clock_to_lock(void) {}


/************************************************************************/
/* Callbacks: uart control                                              */
/************************************************************************/
void core_callback_uart_rx_before_exec(void) {}
void core_callback_uart_rx_after_exec(void) {}
void core_callback_uart_tx_before_exec(void) {}
void core_callback_uart_tx_after_exec(void) {}
void core_callback_uart_cts_before_exec(void) {}
void core_callback_uart_cts_after_exec(void) {}

/************************************************************************/
/* Callbacks: Read app register                                         */
/************************************************************************/
bool core_read_app_register(uint8_t add, uint8_t type)
{
	/* Check if it will not access forbidden memory */
	if (add < APP_REGS_ADD_MIN || add > APP_REGS_ADD_MAX)
		return false;
	
	/* Check if type matches */
	if (app_regs_type[add-APP_REGS_ADD_MIN] != type)
		return false;
	
	/* Receive data */
	(*app_func_rd_pointer[add-APP_REGS_ADD_MIN])();	

	/* Return success */
	return true;
}

/************************************************************************/
/* Callbacks: Write app register                                        */
/************************************************************************/
bool core_write_app_register(uint8_t add, uint8_t type, uint8_t * content, uint16_t n_elements)
{
	/* Check if it will not access forbidden memory */
	if (add < APP_REGS_ADD_MIN || add > APP_REGS_ADD_MAX)
		return false;
	
	/* Check if type matches */
	if (app_regs_type[add-APP_REGS_ADD_MIN] != type)
		return false;

	/* Check if the number of elements matches */
	if (app_regs_n_elements[add-APP_REGS_ADD_MIN] != n_elements)
		return false;

	/* Process data and return false if write is not allowed or contains errors */
	return (*app_func_wr_pointer[add-APP_REGS_ADD_MIN])(content);
}