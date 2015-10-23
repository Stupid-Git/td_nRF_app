
#include "MaikonC.h"

#include "stdbool.h"
#include "string.h"
#include "stdio.h"
#include "stdint.h"

//----- Bogus -----------------------------------------------------------------
/*
#define PRINTF_OVER_UART 1

#if PRINTF_OVER_UART

int PRINTF( const char * format, ... )
{
    int r;
    va_list args;
    va_start (args, format);
    r = vprintf (format, args);
    va_end (args);
    return(r);
}


#else
int PRINTF( const char * format, ... )
{
    int r = 0;
    return(r);
}
#endif
*/


//----- Globals ---------------------------------------------------------------
frameUni_t __uartRxTxUni;
frameUni_t* uartUni_buffer = &__uartRxTxUni;


uint32_t serial_no_co2_logger = 0x11223344;


#define RX_VSTREAM_MAX 4097
uint8_t  g_rxVStream[RX_VSTREAM_MAX];
int g_rxVStream_wp;
int g_rxVStream_rp;

#define TX_VSTREAM_MAX 4097
uint8_t  g_txVStream[TX_VSTREAM_MAX];
int g_txVStream_wp;
int g_txVStream_rp;
//----- Predefines ------------------------------------------------------------




//-----------------------------------------------------------------------------
//
//-----------------------------------------------------------------------------
void maikonC_Init()
{
    memset( uartUni_buffer, 0x00, sizeof(frameUni_t) );
    
    g_rxVStream_wp = 0;
    g_rxVStream_rp = 0;

    g_txVStream_wp = 0;
    g_txVStream_rp = 0;

    serial_no_co2_logger = 0x11223344;
}

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////// stream /////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
int32_t maikonC_rxStreamPush( uint8_t b )
{
    int next_wp;
    next_wp = g_rxVStream_wp + 1;
  //if( next_wp > RX_VSTREAM_MAX )  NG
    if( next_wp >= RX_VSTREAM_MAX ) // if it is equal then it is already out of bounds
        next_wp = 0;
    if( next_wp == g_rxVStream_rp ) //Full
        return(0);

    g_rxVStream[g_rxVStream_wp] = b;
    g_rxVStream_wp = next_wp;
    return(1);
}

int32_t maikonC_rxStreamPop( uint8_t* pb )
{
    if( g_rxVStream_rp == g_rxVStream_wp ) //Empty
        return(0);

    *pb = g_rxVStream[g_rxVStream_rp];
    g_rxVStream_rp++;
  //if( g_rxVStream_rp > RX_VSTREAM_MAX ) NG
    if( g_rxVStream_rp >= RX_VSTREAM_MAX ) // if it is equal then it is already out of bounds
        g_rxVStream_rp = 0;
    return(1);
}
int32_t maikonC_rxStreamPeek( uint8_t* pb )
{
    if( g_rxVStream_rp == g_rxVStream_wp ) //Empty
        return(0);
    *pb = g_rxVStream[g_rxVStream_rp];
    return(1);
}

int32_t maikonC_txStreamPush( uint8_t b )
{
    int next_wp;

    //printf(">%02x", b);

    next_wp = g_txVStream_wp + 1;
  //if( next_wp > TX_VSTREAM_MAX )  NG
    if( next_wp >= TX_VSTREAM_MAX ) // if it is equal then it is already out of bounds
        next_wp = 0;
    if( next_wp == g_txVStream_rp ) //Full
        return(0);

    g_txVStream[g_txVStream_wp] = b;
    g_txVStream_wp = next_wp;
    return(1);
}

int32_t maikonC_txStreamSize()
{
    int32_t size1;
    int32_t size2;
    int32_t size = 0;

    if( g_txVStream_rp == g_txVStream_wp ) //Empty
        return(0);

    if( g_txVStream_rp < g_txVStream_wp )
    {
        size = g_txVStream_wp - g_txVStream_rp;
        return(size);
    }

    if( g_txVStream_wp < g_txVStream_rp )
    {
        size1 = TX_VSTREAM_MAX - g_txVStream_rp;
        size2 = g_txVStream_wp - 0;
        size = size1 + size2;
        return(size);
    }
    return(size);
}


int32_t maikonC_txStreamPop( uint8_t* pb )
{
    if( g_txVStream_rp == g_txVStream_wp ) //Empty
        return(0);

    *pb = g_txVStream[g_txVStream_rp];
    g_txVStream_rp++;
  //if( g_txVStream_rp > TX_VSTREAM_MAX ) NG
    if( g_txVStream_rp >= TX_VSTREAM_MAX ) // if it is equal then it is already out of bounds
        g_txVStream_rp = 0;
    return(1);
}
int32_t maikonC_txStreamPeek( uint8_t* pb )
{
    if( g_txVStream_rp == g_txVStream_wp ) //Empty
        return(0);

    *pb = g_txVStream[g_txVStream_rp];
    return(1);
}


///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////// common /////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
//-----------------------------------------------------------------------------
uint16_t get_checksum(uint8_t *buf, uint16_t rp, uint16_t wp )
{
    uint16_t cs = 0;
    uint16_t idx;
    
    for(idx = rp; idx < wp ; idx++)
        cs += buf[idx];        
    return(cs);
}


///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////// proc01 /////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
//-----------------------------------------------------------------------------
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
int32_t proc01_GetLength(frame01_t* pF)
{
    uint16_t  size;
    uint16_t  dataLen;

    memcpy((void*)&dataLen, &pF->rx.length1, 2);
    size = 5 + dataLen + 2;
    return( size );
}

//-----------------------------------------------------------------------------
int32_t proc01_AddTxCheckSum(frame01_t* pF)
{
    int32_t r;
    int32_t   i;
    uint16_t  size;
    uint16_t  dataLen;
    uint16_t  sum = 0;

    r = 0;

    memcpy((void*)&dataLen, &pF->tx.length1, 2);
    size = dataLen + 7;

    for(i=0; i<size-2; i++)
        sum = sum + pF->byte[i] ;

    //memcpy(&pF->byte[size-2], (void*)&sum, 2) ;
    memcpy(&pF->tx.sum, (void*)&sum, 2) ;

    pF->tx.data[dataLen + 0] = (sum >> 0) & 0x00ff;
    pF->tx.data[dataLen + 1] = (sum >> 8) & 0x00ff;

    return( r );
}

bool  g_Flag_frame01_rx_done = false;
bool  g_Flag_frame01_tx_ready = false;

//-----------------------------------------------------------------------------
int32_t proc01_process_rxframe(frameUni_t* F)
{
    int32_t i;
    int32_t r;
    uint8_t cmd;
    uint8_t subcmd;
    frame01_t* pF;

    uint16_t _size;
    
    pF = &F->frame01;

    r = -1;
    cmd = pF->rx.command1;
    subcmd = pF->rx.command2;

 
    // try and process command
    switch( cmd )
    {

    case READ_SERIAL_NO1_COMMAND1_SERIAL:  //0x58
    case READ_SERIAL_NO2_COMMAND1_SERIAL:  //0xB3

        pF->tx.response = ACK_TX_FRAME_SERIAL;
        pF->tx.length1 = 0x04;
        pF->tx.length2 = 0x00;
        pF->tx.data[0] = (uint8_t) (serial_no_co2_logger      );
        pF->tx.data[1] = (uint8_t) (serial_no_co2_logger >>  8);
        pF->tx.data[2] = (uint8_t) (serial_no_co2_logger >> 16);
        pF->tx.data[3] = (uint8_t) (serial_no_co2_logger >> 24);

        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;

        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;

        
    case CMD_01_0x44:
        cmd_01_44(F);

        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;
        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;
    case CMD_01_0x45:
        cmd_01_45_XXX(F);

        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;
        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;


    case CMD_01_0xF5:  // EmptyFull
        cmd_01_F5_EmptyFull(F);

        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;
        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;


    case CMD_01_0xF8:  //0xF8

        pF->tx.response = ACK_TX_FRAME_SERIAL;
        pF->tx.length1 = 0x42;
        pF->tx.length2 = 0x00;
        for( i = 0 ; i < 0x42 ; i++)
            pF->tx.data[i] = (uint8_t) (i);

        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;

        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;


    case CMD_01_0xF9:  //0xF9
        _size =  (pF->rx.data[1]) * 256;
        _size += (pF->rx.data[0]);
        printf("d1   =  %d, %02x\n",pF->rx.data[1], pF->rx.data[1] );
        printf("d0   =  %d, %02x\n",pF->rx.data[0], pF->rx.data[0] );
        printf("size =  %d, %04x\n", _size, _size);

        pF->tx.response = ACK_TX_FRAME_SERIAL;
        pF->tx.length1 = (_size>>0) & 0x00FF;
        pF->tx.length2 = (_size>>8) & 0x00FF;


        for( i = 0 ; i < _size ; i++)
            pF->tx.data[i] = (uint8_t) (i & 0x000000FF);

        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;

        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;


    default:
        // prepare default Nack
        pF->tx.soh     = SOH_TX_FRAME_SERIAL;
        pF->tx.command = pF->rx.command1;
        pF->tx.response = NAK_TX_FRAME_SERIAL;
        pF->tx.length1 = 0x00;
        pF->tx.length2 = 0x00;

        proc01_AddTxCheckSum(pF); // pF->tx.sum = ...;
        g_Flag_frame01_tx_ready = true;
        F->wrPtr = proc01_GetLength(pF);
        break;
    }
    
    return( r );
}

//-----------------------------------------------------------------------------
// returns :  1 - Complete
//            0 - Not complete
//           -1 - Packet type error
//           -2 - CheckSum error
//-----------------------------------------------------------------------------
int32_t  proc01_checkPacketComplete(uint8_t *buf, uint16_t rp, uint16_t wp )
{
    int16_t size;
    int16_t csum;
    int16_t csum_calc;

    //printf("proc01_checkPacketComplete: buf = 0x%08x, rp = %d, wp = %d\n", (int)buf, rp, wp );

    if( rp ==  wp )
        return(0);
    if( buf[rp + 0] != 0x01 )
        return(-1);
    if( rp + 7 > wp )
        return(0);
    
    size =  buf[rp + 4] << 8;
    size += buf[rp + 3];
    
    if( (rp + 5 + size + 2) > wp)
        return(0);

    csum =  buf[rp + 5 + size + 1] << 8;
    csum += buf[rp + 5 + size + 0];
    csum_calc = get_checksum( buf, rp + 0, rp + 5 + size );
    
    if(csum == csum_calc)
        return(1);
    
    return(-2);
    //return(r);
}

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////// procT2 /////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////

bool g_Flag_frameT2_rx_done = false;
bool g_Flag_frameT2_tx_ready = false;
//-----------------------------------------------------------------------------
int32_t procT2_GetLength(frameT2_t* pF)
{
    uint16_t  size;
    uint16_t  dataLen;

    //memcpy((void*)&dataLen, &pF->length1, 2);
    dataLen = pF->length2 * 256;
    dataLen += pF->length1;
    size = 4 + dataLen + 2;
    return( size );
}

//-----------------------------------------------------------------------------
int32_t procT2_AddTxCheckSum(frameT2_t* pF)
{
    uint32_t i;
    uint16_t dataLen;
    uint16_t sum;
    uint16_t sumIdx;

    dataLen = pF->length2 * 256;
    dataLen += pF->length1;
    sumIdx = dataLen;

    sum = 0;
    for( i=0; i< dataLen; i++)
    {
        sum += pF->data[i];
    }
    pF->sum = sum;
    pF->data[sumIdx + 0] = (sum >> 0) & 0x00ff;
    pF->data[sumIdx + 1] = (sum >> 8) & 0x00ff;
    return(0);
}

//-----------------------------------------------------------------------------
int32_t procT2_process_rxframe(frameUni_t* F)
{
//    int32_t i;
    int32_t r;

    frameT2_t* pF = &F->frameT2;
    r = -1;

    pF->tee ='T';
    pF->two ='2';
    pF->length1 = 0x03;
    pF->length2 = 0x00;
    pF->data[0] = 'x';
    pF->data[1] = 'y';
    pF->data[2] = 'z';
    procT2_AddTxCheckSum(pF);

    r = procT2_GetLength(pF);
    F->wrPtr = r;

    g_Flag_frameT2_tx_ready = true;
    
    return( r );
}

//-----------------------------------------------------------------------------
// returns :  1 - Complete
//            0 - Not complete
//           -1 - Packet type error
//           -2 - CheckSum error
//-----------------------------------------------------------------------------
int32_t  procT2K2_checkPacketComplete(uint8_t *buf, uint16_t rp, uint16_t wp, uint8_t TorK )
{
    int16_t size;
    int16_t csum;
    int16_t csum_calc;

    //printf("procT2K2_checkPacketComplete: buf = 0x%08x, rp = %d, wp = %d, TorK = %c\n", (int)buf, rp, wp, TorK );

    if( (rp + 1) >= wp )
        return(0);
    if( buf[rp + 0] != TorK )
        return(-1);
    if( buf[rp + 1] != '2' )
        return(-1);

    if( rp + 6 > wp )
        return(0);
    
    size =  buf[rp + 3] << 8;
    size += buf[rp + 2];
    
    if( (rp + 4 + size + 2) > wp)
        return(0);

    csum =  buf[rp + 4 + size + 1] << 8;
    csum += buf[rp + 4 + size + 0];
    csum_calc = get_checksum( buf, rp + 4, rp + 4 + size );
    
    if(csum == csum_calc)
        return(1);
    
    return(-2);
}


///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////// procK2 /////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////// procC0 /////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
//-----------------------------------------------------------------------------
// returns :  1 - Complete
//            0 - Not complete
//           -1 - Packet type error
//           -2 - CheckSum error
//-----------------------------------------------------------------------------
int32_t  procC0_checkPacketComplete(uint8_t *buf, uint16_t rp, uint16_t wp )
{
    return(-1);
}



///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////


static int32_t blk_uart_uartRxB_bufferPush(uint8_t c)
{
    int32_t  r;
    r = 1;    
    uartUni_buffer->buffer[uartUni_buffer->wrPtr] = c;
    uartUni_buffer->wrPtr++;
    return(r);
}

/*
static int32_t blk_uart_uartRxS_bufferPush(uint8_t c)
{
    int32_t  r;
    r = 1;    
    uartRxS_buffer[uartRxS_wp] = c;
    uartRxS_wp++;
    return(r);
}
*/


typedef enum epktType
{
    ePkt_Unknown,
    ePkt_0xC0,
    ePkt_0x01,
    ePkt_T2,
    ePkt_K2,
} epktType_t;

epktType_t guess_PktType = ePkt_Unknown;


static int32_t blk_uart_On_UartRx( uint8_t c)
{
    int32_t r;
    r = 0;

    //printf("blk_uart_On_UartRx: c = 0x%02x, wp = %d\n", c, uartUni_buffer->wrPtr);
    if( uartUni_buffer->wrPtr > 20 )
    {
        printf("FAKE HALT\n");
        printf(" FAKE HALT\n");
        printf("  FAKE HALT\n");
        printf("   FAKE HALT\n");

        while(1) ;
    }

    //=================
    //=====  BIG  =====
    //=================
    if( guess_PktType == ePkt_0x01 )
    {
        r = blk_uart_uartRxB_bufferPush( c );    
        r = proc01_checkPacketComplete( uartUni_buffer->buffer, uartUni_buffer->rdPtr, uartUni_buffer->wrPtr );
        if( r == 1)
        {
            g_Flag_frame01_rx_done = true;
            /*
            proc01_process_rxframe( (frame01_t*) &uartRxB_buffer[uartRxB_rp] );
            uartRxB_rp = 0;
            uartRxB_wp = 0;
            */
        }
        if(r < 0)
        {
            printf("01 BAD PACKET\n");
            printf("01 BAD PACKET\n");
        }
    }
    if( guess_PktType == ePkt_T2)
    {
        r = blk_uart_uartRxB_bufferPush( c );    
        r = procT2K2_checkPacketComplete(  uartUni_buffer->buffer, uartUni_buffer->rdPtr, uartUni_buffer->wrPtr, 'T' );
        if( r == 1)
        {
            g_Flag_frameT2_rx_done = true;
            /*
            procT2_process_rxframe( (frameT2_t*) &uartRxB_buffer[uartRxB_rp] );
            uartRxB_rp = 0;
            uartRxB_wp = 0;
            */
        }
        if(r < 0)
        {
            printf("T2 BAD PACKET\n");
            printf("T2 BAD PACKET\n");
        }
    }

    //=================
    //===== SMALL =====
    //=================
    /*
    if( guess_PktType == ePkt_K2)
    {
        r = blk_uart_uartRxS_bufferPush( c );    
        r = procT2K2_checkPacketComplete( uartRxS_buffer, uartRxS_rp, uartRxS_wp, 'K' );
    }
    if( guess_PktType == ePkt_0xC0)
    {
        r = blk_uart_uartRxS_bufferPush( c );    
        r = procC0_checkPacketComplete( uartRxS_buffer, uartRxS_rp, uartRxS_wp );
    }
    */

    
    return(r);
}

int32_t processInputStreamISR()
{
    uint8_t  c;
    int32_t  r;
    static uint8_t m_uartRx_c0 = 0;
    static uint8_t m_uartRx_c1 = 0;
    
    do
    {
        //err_code = app_uart_get(&c);
        //if( err_code != NRF_SUCCESS)
        //    break;
        r = maikonC_rxStreamPop( &c );
        if( r != 1 )
            break;
        r = 0;
        if(guess_PktType == ePkt_Unknown)
        {
            m_uartRx_c1 = m_uartRx_c0;
            m_uartRx_c0 = c;            
            if(                          m_uartRx_c0 == 0xC0  )
            {
                guess_PktType = ePkt_0xC0;
                blk_uart_On_UartRx( 0xC0 );
            }
            if(                          m_uartRx_c0 == 0x01  )
            {
                guess_PktType = ePkt_0x01;
                blk_uart_On_UartRx( 0x01 );
            }
            if( (m_uartRx_c1 == 'T') && (m_uartRx_c0 == '2')  )
            {
                guess_PktType = ePkt_T2;
                blk_uart_On_UartRx( 'T' ); blk_uart_On_UartRx( '2' );
            }
            if( (m_uartRx_c1 == 'K') && (m_uartRx_c0 == '2')  )
            {
                guess_PktType = ePkt_K2;
                blk_uart_On_UartRx( 'K' ); blk_uart_On_UartRx( '2' );
            }
        }
        else
        {
            r = blk_uart_On_UartRx( c );
            if( r==1 )
                break;
        }
        
    } while( 1 );
    
    return(42);//( r );
}


int32_t proc_otherProcessing()
{
    int32_t r = 0;
    return(r);
}


int32_t pullTxBuffer(uint8_t *pb)
{
    int32_t r;
    r = 0;
    if(uartUni_buffer->rdPtr < uartUni_buffer->wrPtr )
    {
        *pb = uartUni_buffer->buffer[uartUni_buffer->rdPtr++];
        r = 1;
    }
    if( uartUni_buffer->rdPtr == uartUni_buffer->wrPtr )
    {
        uartUni_buffer->rdPtr = 0;
        uartUni_buffer->wrPtr = 0;
    }

    return(r);
}

int32_t processOutputStreamISR()
{
    int32_t r = 0;
    uint8_t  c;
    int32_t debugCount = 0;

    if( g_Flag_frame01_tx_ready )
    {
        do
        {
            r = pullTxBuffer( &c );
            if( r == 1 )
            {
                debugCount++;
                r = maikonC_txStreamPush( c );
                if( r == 0 )
                    break;
            }
            else
            {
                uartUni_buffer->rdPtr = 0;
                uartUni_buffer->wrPtr = 0;
                g_Flag_frame01_tx_ready = false;
            }
        } while( r != 0);
    }
    
    if( g_Flag_frameT2_tx_ready )
    {
        do
        {
            r = pullTxBuffer( &c );
            if( r == 1 )
            {
                debugCount++;
                r = maikonC_txStreamPush( c );
                if( r == 0 )
                    break;
            }
            else
            {
                uartUni_buffer->rdPtr = 0;
                uartUni_buffer->wrPtr = 0;
                g_Flag_frameT2_tx_ready = false;
            }
        } while( r != 0);
    }

    printf("processOutputStreamISR: debugCount = %d", debugCount );
    printf(", rp = %d, wp = %d, MAX = %d\n", g_txVStream_rp, g_txVStream_wp, TX_VSTREAM_MAX);

    //TX_VSTREAM_MAX
    //g_txVStream_rp;
    //g_txVStream_wp

    return(r);
}

//=============================================================================
int maikonC_main_proc(void)
{
    int r = 0;

    r = processInputStreamISR();

    if( g_Flag_frame01_rx_done )
    {
        g_Flag_frame01_rx_done = false;
        guess_PktType = ePkt_Unknown;
        proc01_process_rxframe( uartUni_buffer );
    }

    if( g_Flag_frameT2_rx_done )
    {
        g_Flag_frameT2_rx_done = false;
        guess_PktType = ePkt_Unknown;
        procT2_process_rxframe( uartUni_buffer );
    }
    

    proc_otherProcessing();

    r = processOutputStreamISR();


    return(r);
}




//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======
//===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD ===== OLD =======





typedef enum eRxState_e
{
    //rx_IDLE,
    rx_waitSOH,
    rx_waitCMD,
    rx_waitSUBCMD,
    rx_waitL1,
    rx_waitL2,
    rx_waitDATA,
    rx_waitCS0,
    rx_waitCS1,
    rx_gotPkt,
} eRxState_t;
eRxState_t OLD__g_RXsm = rx_waitSOH;

frameUni_t g_frameUni_uart0;
uint16_t   g_frame01_dLen = 0;
uint16_t   g_frame01_dCount;
bool       g_frame01_OVER;
uint16_t   g_frame01_cs;

typedef enum eTxState_e
{
    tx_waitSOH,
    tx_waitCMD,
    tx_waitSUBCMD,
    tx_waitL1,
    tx_waitL2,
    tx_waitDATA,
    tx_waitCS0,
    tx_waitCS1,
    tx_sentPkt,
} eTxState_t;

eTxState_t OLD__g_TXsm = tx_waitSOH;


void OLD__ISR_uart_rx(uint8_t b)
{
    frame01_t *pF = &g_frameUni_uart0.frame01;

    switch( OLD__g_RXsm )
    {
    case rx_waitSOH:
        g_frame01_cs = 0;
        if( b == 0x01) {
            g_frame01_cs += b;
            pF->rx.soh = b;
            OLD__g_RXsm = rx_waitCMD;
        }
        break;

    case rx_waitCMD:
        g_frame01_cs += b;
        pF->rx.command1 = b;
        OLD__g_RXsm = rx_waitSUBCMD;
        break;

    case rx_waitSUBCMD:
        g_frame01_cs += b;
        pF->rx.command2 = b;
        OLD__g_RXsm = rx_waitL1;
        break;

    case rx_waitL1:
        g_frame01_cs += b;
        pF->rx.length1 = b;
        OLD__g_RXsm = rx_waitL2;
        break;

    case rx_waitL2:
        g_frame01_cs += b;
        pF->rx.length2 = b;
        g_frame01_dLen = (pF->rx.length2<<8) + (pF->rx.length1);
        g_frame01_dCount = 0;
        if( g_frame01_dLen > FRAME01_MAXDATALEN )
            g_frame01_OVER = true;
        if( g_frame01_dLen > 0)
            OLD__g_RXsm = rx_waitDATA;
        else
            OLD__g_RXsm = rx_waitCS0;
        break;

    case rx_waitDATA:
        g_frame01_cs += b;
        if( g_frame01_dCount<FRAME01_MAXDATALEN )
            pF->rx.data[g_frame01_dCount] = b;
        g_frame01_dCount++;
        if(g_frame01_dCount == g_frame01_dLen)
            OLD__g_RXsm = rx_waitCS0;
        break;

    case rx_waitCS0:
        //g_frame01_cs += b;
        pF->rx.sum = b;
        OLD__g_RXsm = rx_waitCS1;
        break;

    case rx_waitCS1:
        //g_frame01_cs += b;
        pF->rx.sum += (((uint16_t)(b)) << 8);

        if( g_frame01_cs == pF->rx.sum )
            g_Flag_frame01_rx_done = true;
        //else
        //    g_Flag_frame01_rx_done = false;

        OLD__g_RXsm = rx_gotPkt; //rx_IDLE;
        break;

    case  rx_gotPkt:
        break;

    }
}

int32_t OLD__ISR_uart_tx(uint8_t *pb)
{
    int32_t r = 1;
    frame01_t *pF = &g_frameUni_uart0.frame01; // frame01_t *pF = &g_frame01_uart0;

    switch( OLD__g_TXsm )
    {
    case tx_waitSOH:
        g_frame01_cs = 0;
        *pb = pF->tx.soh;
        g_frame01_cs += *pb;
        OLD__g_TXsm = tx_waitCMD;
        break;

    case tx_waitCMD:
        *pb = pF->tx.command;
        g_frame01_cs += *pb;
        OLD__g_TXsm = tx_waitSUBCMD;
        break;

    case tx_waitSUBCMD:
        *pb = pF->tx.response;
        g_frame01_cs += *pb;
        OLD__g_TXsm = tx_waitL1;
        break;

    case tx_waitL1:
        *pb = pF->tx.length1;
        g_frame01_cs += *pb;
        OLD__g_TXsm = tx_waitL2;
        break;

    case tx_waitL2:
        *pb = pF->tx.length2;
        g_frame01_cs += *pb;
        g_frame01_dLen = (pF->tx.length2<<8) + (pF->tx.length1);
        g_frame01_dCount = 0;
        if( g_frame01_dLen > FRAME01_MAXDATALEN )
            g_frame01_OVER = true;
        if( g_frame01_dLen > 0)
            OLD__g_TXsm = tx_waitDATA;
        else
            OLD__g_TXsm = tx_waitCS0;
        break;

    case tx_waitDATA:
        if( g_frame01_dCount<FRAME01_MAXDATALEN )
            *pb = pF->tx.data[g_frame01_dCount];
        g_frame01_cs += *pb;
        g_frame01_dCount++;
        if(g_frame01_dCount == g_frame01_dLen)
            OLD__g_TXsm = tx_waitCS0;
        break;

    case tx_waitCS0:
        *pb = (pF->tx.sum >> 0) & 0x00FF;
        g_frame01_cs += *pb;
        OLD__g_TXsm = tx_waitCS1;
        break;

    case tx_waitCS1:
        *pb = (pF->tx.sum >> 8) & 0x00FF;
        g_frame01_cs += *pb;
        OLD__g_TXsm = tx_waitCS1;

        //if( g_frame01_cs == pF->tx.sum )
        
        g_Flag_frame01_tx_ready = false;
        OLD__g_TXsm = tx_sentPkt;
        break;

    case tx_sentPkt:
        r = 0;
    }

    return(r);
}

int32_t OLD__processInputStreamISR()
{
    int32_t r;
    uint8_t  c;
    while(OLD__g_RXsm != rx_gotPkt)
    {
        r = maikonC_rxStreamPop( &c );
        if( r == 0 )
            break;
        if( r == 1 )
            OLD__ISR_uart_rx( c );
    }
    return(r);
}

int32_t OLD__proc_otherProcessing()
{
    int32_t r = 0;
//    uint8_t  c;

    return(r);
}


int32_t OLD__processOutputStreamISR()
{
    int32_t r = 0;
    uint8_t  c;

    if( g_Flag_frame01_tx_ready )
    {
        do
        {
            r = OLD__ISR_uart_tx( &c );
            if( r == 1 )
            {
                r = maikonC_txStreamPush( c );
                if( r == 0 )
                    break;
            }
            else
            {
                OLD__g_RXsm = rx_waitSOH; // Enable RX again
            }
        } while( r != 0);
    }
    return(r);
}

//=============================================================================
int OLD__maikonC_main_proc(void)
{
    int r = 0;

    r = OLD__processInputStreamISR();

    if( g_Flag_frame01_rx_done ) 
    {
        g_Flag_frame01_rx_done = false;
        proc01_process_rxframe( &g_frameUni_uart0 );
    }

    OLD__proc_otherProcessing();

    r = OLD__processOutputStreamISR();


    return(r);
}
 