#ifndef _STRUCTS_H_
#define _STRUCTS_H


typedef struct
{
	uint16_t period_bnc0, period_bnc1, period_signal_a, period_signal_b;
    uint16_t tail_bnc0, tail_bnc1, tail_signal_a, tail_signal_b;
    uint16_t t_bnc0, t_bnc1, t_signal_a, t_signal_b;
    uint16_t count_pulses_bnc0, count_pulses_bnc1, count_pulses_signal_a, count_pulses_signal_b; 
} countdown_t;


typedef struct 
{
   uint16_t on_ms, off_ms;
   uint16_t pulses;
   uint16_t tail_ms;
} interval_t;


typedef struct{

    bool bnc_0, bnc_1, signal_a, signal_b;
} ports_state_t;





#endif /* _STRUCT_H_ */