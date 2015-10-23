
#include "MaikonC.h"


#include "stdbool.h"
#include "string.h"
#include "stdio.h"
#include "stdint.h"

#define SET true
#define CLR false

#define MAX_NUM_REC_DATA_REC 8000

typedef struct records_s
{
    uint16_t ch1;
    uint16_t ch2;
    uint16_t ch3;
    uint16_t ch4;
}records_t;

records_t record_data[8192];
uint16_t ch2;

uint16_t num_rec_data_rec;

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//----- Serial ----------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------

//struct def_state_ctrl_com state_ctrl_com;

//uint16_t  time_out_count_rx_serial_com;
//uint16_t  time_out_count_byte_serial_com;
//uint8_t wait_radio_serial_com;
uint16_t  num_read_rec_data_serial_com;
uint16_t Lnum_read_byte_data_serial_com; //# of Bytes = #rec * (2 * 3chan = BYTE_ALLCH_READ)
uint16_t  memory_next_rec_offset_serial;
uint8_t block_no_serial_com;
uint16_t  count_read_rec_data_serial_com;
uint16_t  num_read_rec_data_block_serial_com; // for reading records
//uint16_t  num_read_byte_data_block_serial_com; // for reading bytes

void cmd_01_44(frameUni_t* F) //      (CMD1 == REQ_READ_REC_DATA_NEW_COMMAND1_SERIAL ) ||//0x44
{
    frame01_t* pF;    
    pF = &F->frame01;

    switch (pF->rx.command2)
    {
    case REQ_READ_REC_DATA_NEW1_COMMAND2_SERIAL:

        //FAKE

        block_no_serial_com = 0;
        Lnum_read_byte_data_serial_com = ((uint16_t) pF->rx.data[0] + ((uint16_t) pF->rx.data[1] << 8));
        if (Lnum_read_byte_data_serial_com == 0)
        {
            num_read_rec_data_serial_com = num_rec_data_rec;
        }
        else
        if (Lnum_read_byte_data_serial_com > (num_rec_data_rec * 8/*BYTE_ALLCH_READ*/))
        {
            num_read_rec_data_serial_com = num_rec_data_rec;
        }
        else
        {
            num_read_rec_data_serial_com = Lnum_read_byte_data_serial_com / 8/*BYTE_ALLCH_READ*/;
        }
        
        printf("\n");
        printf("Maikon: cmd_01_44: num_rec_data_rec               = %d\n", num_rec_data_rec);
        printf("Maikon: cmd_01_44: Lnum_read_byte_data_serial_com = %d\n", Lnum_read_byte_data_serial_com);
        printf("Maikon: cmd_01_44: num_read_rec_data_serial_com   = %d\n", num_read_rec_data_serial_com);
        printf("Maikon: cmd_01_44: block_no_serial_com            = %d\n", block_no_serial_com);

        /*TODO
        Lnum_read_byte_data_serial_com = ((uint16_t) pF->rx.data[0] + ((uint16_t) pF->rx.data[1] << 8));
        if (Lnum_read_byte_data_serial_com == 0)
        {
            num_read_rec_data_serial_com = num_rec_data_rec;
        }
        else
        if (Lnum_read_byte_data_serial_com > (num_rec_data_rec * BYTE_ALLCH_READ))
        {
            num_read_rec_data_serial_com = num_rec_data_rec;
        }
        else
        {
            num_read_rec_data_serial_com = Lnum_read_byte_data_serial_com / BYTE_ALLCH_READ;
        }
        
        // 検査モードの時は8064個（EEPROMに記録できる全データ）に変更する
        if(FLAG_TEST_CO2_LOGGER == SET_CO2_LOGGER){
            num_read_rec_data_serial_com = 8064;
        }
        else;
        // 吸い上げ開始記録データアドレスを記憶する
        memorize_oldest_rec(num_read_rec_data_serial_com);
        block_no_serial_com = 0;
        TODO*/
        // 応答を送信開始
        pF->tx.length1 = 0x40;
        pF->tx.length2 = 0x00;
        pF->tx.response = ACK_TX_FRAME_SERIAL;
        /*TODO
        if (pF->rx.command1 == REQ_READ_REC_DATA_NEW_COMMAND1_SERIAL) {
            pF->tx.data[0]  = (uint8_t) start_rec.interval_rec;
            pF->tx.data[1]  = (uint8_t)(start_rec.interval_rec >> 8);
            pF->tx.data[2]  = start_rec.time_start[0];
            pF->tx.data[3]  = start_rec.time_start[1];
            pF->tx.data[4]  = start_rec.time_start[2];
            pF->tx.data[5]  = start_rec.time_start[3];
            pF->tx.data[6]  = start_rec.time_start[4];
            pF->tx.data[7]  = start_rec.time_start[5];
            pF->tx.data[8]  = start_rec.time_start[6];
            pF->tx.data[9]  = start_rec.time_start[7];
            pF->tx.data[10] = start_rec.way_start;
            pF->tx.data[11] = start_rec.state_recmode;
        }
        else {
            pF->tx.data[0]  = (uint8_t) rec.interval_rec;
            pF->tx.data[1]  = (uint8_t)(rec.interval_rec >> 8);
            pF->tx.data[2]  = rec.time_start[0];
            pF->tx.data[3]  = rec.time_start[1];
            pF->tx.data[4]  = rec.time_start[2];
            pF->tx.data[5]  = rec.time_start[3];
            pF->tx.data[6]  = rec.time_start[4];
            pF->tx.data[7]  = rec.time_start[5];
            pF->tx.data[8]  = rec.time_start[6];
            pF->tx.data[9]  = rec.time_start[7];
            pF->tx.data[10] = rec.way_start;
            pF->tx.data[11] = rec.state_recmode;
        }
        pF->tx.data[12] = (uint8_t) (elapsed_time_last_rec_rec      );
        pF->tx.data[13] = (uint8_t) (elapsed_time_last_rec_rec >>  8);
        pF->tx.data[14] = (uint8_t) (elapsed_time_last_rec_rec >> 16);
        pF->tx.data[15] = (uint8_t) (elapsed_time_last_rec_rec >> 24);
        pF->tx.data[16] = interval_monitor_mon;
        pF->tx.data[17] =          set_warn.co2.time;
        pF->tx.data[18] = (uint8_t)  set_warn.co2.under;
        pF->tx.data[19] = (uint8_t) (set_warn.co2.under >> 8);
        pF->tx.data[20] = (uint8_t)  set_warn.co2.over;
        pF->tx.data[21] = (uint8_t) (set_warn.co2.over >> 8);
        pF->tx.data[22] =          set_warn.temp.time;
        pF->tx.data[23] = (uint8_t)  set_warn.temp.under;
        pF->tx.data[24] = (uint8_t) (set_warn.temp.under >> 8);
        pF->tx.data[25] = (uint8_t)  set_warn.temp.over;
        pF->tx.data[26] = (uint8_t) (set_warn.temp.over >> 8);
        pF->tx.data[27] =          set_warn.rh.time;
        pF->tx.data[28] = (uint8_t)  set_warn.rh.under;
        pF->tx.data[29] = (uint8_t) (set_warn.rh.under >> 8);
        pF->tx.data[30] = (uint8_t)  set_warn.rh.over;
        pF->tx.data[31] = (uint8_t) (set_warn.rh.over >> 8);
        pF->tx.data[32] = 0x00;
        pF->tx.data[33] = 0x00;
        pF->tx.data[34] = 0x00;
        pF->tx.data[35] = 0x00;
        pF->tx.data[36] = 0x00;
        //      pF->tx.data[37] = (uint8_t)  lowEnergyMode;
        pF->tx.data[37] = (uint8_t)( (lowEnergyMode == 1) || (state_LEM != STATE_LEM_0) );
        pF->tx.data[38] = (uint8_t)  set_warn.free22;
        pF->tx.data[39] = (uint8_t)  set_warn.free23;
        pF->tx.data[40] = (uint8_t)  set_warn.free24;
        pF->tx.data[41] = unit_temp_disp;
        pF->tx.data[42] = state_upper_ch_disp;
        pF->tx.data[43] = setting_multi_ch_disp;
        pF->tx.data[44] = setting_ctrl_lock_sw;
        pF->tx.data[45] = (uint8_t) (serial_no_co2_logger      );
        pF->tx.data[46] = (uint8_t) (serial_no_co2_logger >>  8);
        pF->tx.data[47] = (uint8_t) (serial_no_co2_logger >> 16);
        pF->tx.data[48] = (uint8_t) (serial_no_co2_logger >> 24);
        pF->tx.data[49] =   ((power_data_ac.level  << 4) & 0xF0) | ((power_data_bat.level     ) & 0x0F);
        pF->tx.data[50] = INFO_CH1_CO2_LOGGER;
        pF->tx.data[51] = INFO_CH2_CO2_LOGGER;

        if( ch3_zokusei_serial == HD_SER ) // （0xD1:0.1% 湿度%） //+RTR 0x0008
            pF->tx.data[52] = HD_SER; //+RTR 0x0008
        else //+RTR 0x0008
            if( ch3_zokusei_serial == NORM_SER ) // （0xD1:1.0% 湿度%） //+RTR 0x0008
                pF->tx.data[52] = NORM_SER; //+RTR 0x0008
            else //+RTR 0x0008
                pF->tx.data[52] = NORM_SER; // 未接続などの場合は標準センサとします。               //+RTR 0x0008

        pF->tx.data[53] = INFO_CH4_CO2_LOGGER;
        pF->tx.data[54] = 0x00;
        pF->tx.data[55] = 0x00;
        pF->tx.data[56] = (uint8_t)( index_rec_data_rec     );
        pF->tx.data[57] = (uint8_t)( index_rec_data_rec >> 8);
        pF->tx.data[58] = (uint8_t)( (num_read_rec_data_serial_com * BYTE_ALLCH_READ)     );
        pF->tx.data[59] = (uint8_t)( (num_read_rec_data_serial_com * BYTE_ALLCH_READ) >> 8);
        pF->tx.data[60] = (uint8_t)( (num_rec_data_rec * BYTE_ALLCH_READ)     );
        pF->tx.data[61] = (uint8_t)( (num_rec_data_rec * BYTE_ALLCH_READ) >> 8);
        pF->tx.data[62] = (uint8_t)( code_product_co2_logger     );
        pF->tx.data[63] = (uint8_t)( code_product_co2_logger >> 8);
        TODO*/
        break;

    case REQ_READ_REC_DATA_NEW2_COMMAND2_SERIAL:
        // 応答を送信開始
        pF->tx.length1 = 0x40;
        pF->tx.length2 = 0x00;
        pF->tx.response = ACK_TX_FRAME_SERIAL;
        /*TODO
        for (i_global = 0; i_global < 16; i_global++) {
            pF->tx.data[i_global +  0] = name_ch_co2_logger.ch1[i_global];
        }
        for (i_global = 0; i_global < 16; i_global++) {
            pF->tx.data[i_global + 16] = name_ch_co2_logger.ch2[i_global];
        }
        for (i_global = 0; i_global < 16; i_global++) {
            pF->tx.data[i_global + 32] = name_ch_co2_logger.ch3[i_global];
        }
        for (i_global = 0; i_global < 16; i_global++) {
            // ch 4 is set and returned but the DLL will only handle the first 3 ch
            pF->tx.data[i_global + 48] = name_ch_co2_logger.ch4[i_global]; // Unused
        }
        TODO*/
        break;

    case REQ_READ_REC_DATA_NEW3_COMMAND2_SERIAL:
        // 応答を送信開始
        pF->tx.length1 = 0x40;
        pF->tx.length2 = 0x00;
        pF->tx.response = ACK_TX_FRAME_SERIAL;
        /*TODO
        for (i_global = 0; i_global < 8; i_global++) {
            pF->tx.data[i_global] = co2_radio_com.name_group[i_global];
        }
        for (i_global = 0; i_global < 8; i_global++) {
            pF->tx.data[i_global + 8] = co2_radio_com.name[i_global];
        }
        pF->tx.data[16] = co2_radio_com.no;
        pF->tx.data[17] = co2_radio_com.frequency;
        for (i_global = 0; i_global < 8; i_global++) {
            pF->tx.data[i_global + 18] = temp_rh_radio_com.name_group[i_global];
        }
        for (i_global = 0; i_global < 8; i_global++) {
            pF->tx.data[i_global + 26] = temp_rh_radio_com.name[i_global];
        }
        pF->tx.data[34] = temp_rh_radio_com.no;
        pF->tx.data[35] = temp_rh_radio_com.frequency;
        //REF
        //if(code_product_co2_logger == RTR574_VL_LOGGER){
        //pF->tx.data[36] = protect_radio_com;
        //pF->tx.data[37] = state_ctrl_com.radio;
        //pF->tx.data[38] = (uint8_t)(f_current_value_intuv);
        //pF->tx.data[39] = (uint8_t)(f_current_value_intuv >> 8);
        //}
        //else{
        //pF->tx.data[36] = (uint8_t)(f_current_value_intuv);
        //pF->tx.data[37] = (uint8_t)(f_current_value_intuv >> 8);
        //pF->tx.data[38] = state_ctrl_com.irda;
        //pF->tx.data[39] = 0x00;
        //}
        //REF

        if( pF->rx.command1 == REQ_READ_REC_DATA_NEW_COMMAND1_SERIAL ) {
            pF->tx.data[36] = protect_radio_com;
        } else {
            pF->tx.data[36] = protect_radio_com;
        }
        if (isON_RADIO_COM == ON_RADIO_COM) {  // P40 無線   1=ON, 0=OFF
            pF->tx.data[37] = 0x01; // 1=>受信許可
        } else {
            pF->tx.data[37] = 0x00;
        }
        if (ReadPortDataP36() == SET_INTP1X) {  // IrDA 1=OFF, 0=ON
            pF->tx.data[38] = 0x00;
        } else {
            pF->tx.data[38] = 0x01; // 1=>受信許可
        }
        pF->tx.data[39] = 0x00; // Reserved

        for(i_global = 0; i_global < 4; i_global++) {
            pF->tx.data[i_global + 40] = date_user_cal_CO2_sensor[i_global];
        }
        pF->tx.data[44] = (uint8_t)( slope_user_cal_CO2_sensor           );
        pF->tx.data[45] = (uint8_t)((slope_user_cal_CO2_sensor)      >> 8);
        pF->tx.data[46] = (uint8_t)( intercept_user_cal_CO2_sensor       );
        pF->tx.data[47] = (uint8_t)((intercept_user_cal_CO2_sensor)  >> 8);
        pF->tx.data[48] = 0x00;
        pF->tx.data[49] = 0x00;
        pF->tx.data[50] = 0x00;
        pF->tx.data[51] = 0x00;
        for(i_global = 0; i_global < 4; i_global++) {
            pF->tx.data[i_global + 52] = date_user_cal_TEMP_RH_sensor[i_global];
        }
        pF->tx.data[56] = (uint8_t)( slope_user_cal_TEMP_sensor          );
        pF->tx.data[57] = (uint8_t)((slope_user_cal_TEMP_sensor)     >> 8);
        pF->tx.data[58] = (uint8_t)( intercept_user_cal_TEMP_sensor      );
        pF->tx.data[59] = (uint8_t)((intercept_user_cal_TEMP_sensor) >> 8);
        pF->tx.data[60] = (uint8_t)( slope_user_cal_RH_sensor            );
        pF->tx.data[61] = (uint8_t)((slope_user_cal_RH_sensor)       >> 8);
        pF->tx.data[62] = (uint8_t)( intercept_user_cal_RH_sensor        );
        pF->tx.data[63] = (uint8_t)((intercept_user_cal_RH_sensor)   >> 8);
        TODO*/
        break;
    default:
        pF->tx.response = NAK_TX_FRAME_SERIAL;
        pF->tx.length1 = 0x00;
        pF->tx.length2 = 0x00;
    }
}

void IC_read_rec(unsigned int start_offset, unsigned int end_offset, unsigned char kind, unsigned char* ptr_dest)
{

}

void cmd_01_45_XXX(frameUni_t* F) //READ_REC_DATA_NEW_COMMAND1_SERIAL
{
    frame01_t* pF;    
    pF = &F->frame01;


    switch(pF->rx.command2)
    {
    case READ_REC_DATA_NEW_COMMAND2_SERIAL: //0x06
        break;

        //case REREAD_REC_DATA_OLD_COMMAND2_SERIAL:
    default:
        if(block_no_serial_com != 0){
            block_no_serial_com--;
        }
        else;
        break;
    }




    if((block_no_serial_com * 128) < num_read_rec_data_serial_com)
    {
        if((((block_no_serial_com + 1) * 128) - 1) < num_read_rec_data_serial_com)
            num_read_rec_data_block_serial_com = 128;
        else
            num_read_rec_data_block_serial_com = num_read_rec_data_serial_com - (block_no_serial_com * 128);

        //In READ_REC_DATA_OLD_COMMAND1_SERIAL above, really only this line is different
        //TODO
//TODO        IC_read_rec(num_read_rec_data_serial_com - (block_no_serial_com * 128) - num_read_rec_data_block_serial_com, (num_read_rec_data_serial_com - 1) - (block_no_serial_com * 128), ALL_OPPOSITE_READ_REC, &pF->tx.data[0]);
        pF->tx.data[0] = ((block_no_serial_com>>0) & 0x00FF);
        pF->tx.data[1] = ((block_no_serial_com>>8) & 0x00FF);
    }
    else{
        num_read_rec_data_block_serial_com = 0;
    }
    block_no_serial_com++;

    printf("Maikon: cmd_01_45: block_no_serial_com                = %d\n", block_no_serial_com);
    printf("Maikon: cmd_01_45: num_read_rec_data_serial_com       = %d\n", num_read_rec_data_serial_com);
    printf("Maikon: cmd_01_45: num_read_rec_data_block_serial_com = %d\n", num_read_rec_data_block_serial_com);
            

    // 応答を送信開始
    pF->tx.length1 = (uint8_t)((num_read_rec_data_block_serial_com * 8 /*BYTE_ALLCH_READ*/)     );
    pF->tx.length2 = (uint8_t)((num_read_rec_data_block_serial_com * 8 /*BYTE_ALLCH_READ*/) >> 8);
    pF->tx.response = block_no_serial_com;

}

void cmd_01_F5_EmptyFull(frameUni_t* F)
{
    uint16_t recCount;
    frame01_t* pF;    
    pF = &F->frame01;

    recCount = ((uint16_t) pF->rx.data[0] + ((uint16_t) pF->rx.data[1] << 8));

    if(recCount > MAX_NUM_REC_DATA_REC)
        recCount = MAX_NUM_REC_DATA_REC;

    num_rec_data_rec = recCount;

    /*
    if(F->frame01.rx.data[0] == 0)
        num_rec_data_rec = 0;
    else
        num_rec_data_rec = MAX_NUM_REC_DATA_REC;
    */
    /*
    EVENT_NORM_DISP_CHENGED_NUM_REC_DATA = SET;
    */          
    F->frame01.tx.response = ACK_TX_FRAME_SERIAL;
    F->frame01.tx.length1 = 0x02;
    F->frame01.tx.length2 = 0x00;
    F->frame01.tx.data[0] = ((num_rec_data_rec>>0) & 0x00FF);
    F->frame01.tx.data[1] = ((num_rec_data_rec>>8) & 0x00FF);
};

void serial_proc()
{
}


//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//----- Measure ---------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
uint8_t seconds_to_meas_sensors = 0;
uint8_t interval_meas_sensors = 1;
bool EVENT_SENSOR_START_READ_XXX = CLR;
bool EVENT_SENSOR_DATA_READ_XXX = CLR;
bool EVENT_SENSOR_DATA_STORE_XXX = CLR;

bool STATUS_SENSOR_DATA_READY_XXX = CLR; // Just starting an access, so Clear Ready

uint16_t m_chx_bogus_data = 40;
uint16_t m_ch1_data;
uint16_t m_ch2_data;
uint16_t m_ch3_data;
uint16_t m_ch4_data;

void measure_1Sec()
{
    if(seconds_to_meas_sensors == 0)  // If the time has come to measure the sensor
    {
        seconds_to_meas_sensors = interval_meas_sensors;
        EVENT_SENSOR_START_READ_XXX = SET;
        STATUS_SENSOR_DATA_READY_XXX = CLR; // Just starting an access, so Clear Ready
    }
    seconds_to_meas_sensors--;
}

void measure_proc()
{

    if( EVENT_SENSOR_START_READ_XXX )
    {
        EVENT_SENSOR_START_READ_XXX = CLR;
        EVENT_SENSOR_DATA_READ_XXX = SET;
    }
    else
        if( EVENT_SENSOR_DATA_READ_XXX )
        {
            EVENT_SENSOR_DATA_READ_XXX = CLR;

            m_chx_bogus_data += 10;
            if(m_chx_bogus_data > 80)
                m_chx_bogus_data = 40;

            EVENT_SENSOR_DATA_STORE_XXX = SET;
        }
        else
            if( EVENT_SENSOR_DATA_STORE_XXX )
            {
                EVENT_SENSOR_DATA_STORE_XXX = CLR;

                m_ch1_data = m_chx_bogus_data + 1;
                m_ch2_data = m_chx_bogus_data + 2;
                m_ch3_data = m_chx_bogus_data + 3;
                m_ch4_data = m_chx_bogus_data + 4;

                STATUS_SENSOR_DATA_READY_XXX = SET;
            }
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//----- Record ----------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
bool EVENT_REC_START_RECORDING = CLR;


#define  ENDLESS_RECMODE_REC 0x00
#define  ONETIME_RECMODE_REC 0x80

struct def_table_rec{
    uint16_t  interval_rec;
    uint8_t   time_start[8];
    uint8_t   way_start;
    uint8_t   state_recmode;
    uint32_t  time_to_rec_start;
};

struct def_table_rec rec;


uint16_t  m_recordsIdx;
uint16_t m_elapsed_time_since_last_record;

void record_1Sec()
{
    m_elapsed_time_since_last_record++;
}

void record_proc()
{

    if(EVENT_REC_START_RECORDING)
    {
        if( STATUS_SENSOR_DATA_READY_XXX )
        {
            EVENT_REC_START_RECORDING = CLR;

            switch (rec.state_recmode) {
            case ONETIME_RECMODE_REC:
                if (m_recordsIdx < MAX_NUM_REC_DATA_REC) {

                    record_data[m_recordsIdx].ch1 = m_ch1_data;
                    record_data[m_recordsIdx].ch2 = m_ch2_data;
                    record_data[m_recordsIdx].ch3 = m_ch3_data;
                    record_data[m_recordsIdx].ch4 = m_ch4_data;

                    m_recordsIdx++;

                    num_rec_data_rec++;
                    if (num_rec_data_rec > MAX_NUM_REC_DATA_REC)
                        num_rec_data_rec = MAX_NUM_REC_DATA_REC;

                    m_elapsed_time_since_last_record = 0;
                } else { // EEPROM is FULL
                }
                break;

            default:
                record_data[m_recordsIdx].ch1 = m_ch1_data;
                record_data[m_recordsIdx].ch2 = m_ch2_data;
                record_data[m_recordsIdx].ch3 = m_ch3_data;
                record_data[m_recordsIdx].ch4 = m_ch4_data;

                m_recordsIdx++;
                if (m_recordsIdx > MAX_NUM_REC_DATA_REC)
                    m_recordsIdx = 0;

                num_rec_data_rec++;
                if (num_rec_data_rec > MAX_NUM_REC_DATA_REC)
                    num_rec_data_rec = MAX_NUM_REC_DATA_REC;

                m_elapsed_time_since_last_record = 0;
                break;
            } // switch
        }
    }
}

//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//----- Display ---------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
bool EVENT_DISPLAY_UPDATE_DISPLAY = CLR;

uint16_t m_ch1_display_data;
uint16_t m_ch2_display_data;
uint16_t m_ch3_display_data;
uint16_t m_ch4_display_data;

void display_1Sec()
{
}

void display_proc()
{
    if( EVENT_DISPLAY_UPDATE_DISPLAY )
    {
        EVENT_DISPLAY_UPDATE_DISPLAY = CLR;
        if( STATUS_SENSOR_DATA_READY_XXX )
        {
            m_ch1_display_data =  m_ch1_data;
            m_ch2_display_data =  m_ch2_data;
            m_ch3_display_data =  m_ch3_data;
            m_ch4_display_data =  m_ch4_data;
        }
    }
}


//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//----- Main ------------------------------------------------------------------
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
void main_Init()
{
    //BOGUS
    num_rec_data_rec = 300;
    m_recordsIdx = 300;

}

void main_Loop()
{

    serial_proc();
    measure_proc();
    record_proc();
    display_proc();

}

void main_IntCt()
{
    measure_1Sec();
    record_1Sec();
    display_1Sec();
}



