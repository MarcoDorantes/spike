using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace UnitTestProject1
{
  [TestClass]
  public class fix_specs
  {
    [TestMethod]
    public void add_as_tags_come()
    {
      IEnumerable<List<KeyValuePair<int,string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,710,1050,", line.ElementAt(0));
      Assert.AreEqual<string>("y,2,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,123,", line.ElementAt(2));
    }
    [TestMethod]
    public void add_as_tags_come2()
    {
      IEnumerable<List<KeyValuePair<int, string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"8"), new KeyValuePair<int, string>(22, "X2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,22,710,1050,", line.ElementAt(0));
      Assert.AreEqual<string>("8,X2,,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,2,,", line.ElementAt(2));
      Assert.AreEqual<string>("y,,,123,", line.ElementAt(3));
    }
    [TestMethod]
    public void add_as_tags_come_rgroup()
    {
      IEnumerable<List<KeyValuePair<int, string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"8"), new KeyValuePair<int, string>(22, "X2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(711, "1"), new KeyValuePair<int, string>(361, "A"), new KeyValuePair<int, string>(362, "Z") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,22,710,1050,711,361,362,", line.ElementAt(0));
      Assert.AreEqual<string>("8,X2,,,,,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,2,,,,,", line.ElementAt(2));
      Assert.AreEqual<string>("y,,,123,,,,", line.ElementAt(3));
      Assert.AreEqual<string>("y,,,,1,A,Z,", line.ElementAt(4));
    }
    [TestMethod]
    public void add_as_tags_come_rgroups()
    {
      IEnumerable<List<KeyValuePair<int, string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"8"), new KeyValuePair<int, string>(22, "X2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(711, "2"), new KeyValuePair<int, string>(361, "A1"), new KeyValuePair<int, string>(362, "Z1"), new KeyValuePair<int, string>(361, "A2"), new KeyValuePair<int, string>(362, "Z2") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,22,710,1050,711,361,362,361,362,", line.ElementAt(0));
      Assert.AreEqual<string>("8,X2,,,,,,,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,2,,,,,,,", line.ElementAt(2));
      Assert.AreEqual<string>("y,,,123,,,,,,", line.ElementAt(3));
      Assert.AreEqual<string>("y,,,,2,A1,Z1,A2,Z2,", line.ElementAt(4));
    }
    IEnumerable<string> to_tabular(IEnumerable<List<KeyValuePair<int, string>>> fixlog)
    {
      var heads = to_tabular_heads(fixlog);
      var heads_line = heads.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      yield return heads_line;

      int distinct_tag_count = heads.Count;
      foreach (var msg in fixlog)
      {
        string[] line = new string[distinct_tag_count];
        foreach (var pair in msg)
        {
          int repeated_tag_count_in_msg = msg.Count(p => p.Key == pair.Key);
          if (repeated_tag_count_in_msg == 1)
          {
            int index = heads.IndexOf(pair.Key);
            line[index] = pair.Value;
          }
          else
          {
            int start_index = 0;
            do
            {
              int index = heads.FindIndex(start_index, t => t == pair.Key);
              if (line[index] == null)
              {
                line[index] = pair.Value;
                break;
              }
              else
              {
                start_index = index + 1;
              }
            } while (start_index < distinct_tag_count);
          }
        }
        yield return line.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      }
    }
    List<int> to_tabular_heads(IEnumerable<List<KeyValuePair<int, string>>> fixlog)
    {
      return fixlog.Aggregate(new List<int>(), (whole, msg) =>
      {
        msg.Aggregate(whole, (same_whole, pair) =>
        {
          if (same_whole.IndexOf(pair.Key) == -1)
          {
            same_whole.Add(pair.Key);
          }
          else
          {
            int repeated_tag_count_in_msg = msg.Count(p => p.Key == pair.Key);
            if (repeated_tag_count_in_msg > 1)
            {
              same_whole.Add(pair.Key);
            }
          }
          return same_whole;
        });
        return whole;
      });
    }

    IEnumerable<string> read1(System.IO.FileInfo file)
    {
      using (var reader = file.OpenText())
      {
        do
        {
          string line = reader.ReadLine();
          if (line == null) yield break;
          if (string.IsNullOrWhiteSpace(line)) continue;
          yield return line;
        } while (true);
      }
    }

    IEnumerable<string> read2(System.IO.FileInfo file)
    {
      //return read2(file.OpenText());
      foreach (var line in read2(file.OpenText()))
      {
        yield return line;
      }
    }

    IEnumerable<string> read2(System.IO.TextReader reader)
    {
      using (reader)
      {
        do
        {
          string line = reader.ReadLine();
          if (line == null) yield break;
          if (string.IsNullOrWhiteSpace(line)) continue;
          yield return line;
        } while (true);
      }
    }

    [TestMethod]
    public void read1_()
    {
      var values = new string[] { "one", "two" };

      var file = new System.IO.FileInfo(System.IO.Path.GetTempFileName());
      Trace.WriteLine(file.FullName);
      try
      {
        using (var writer = new System.IO.StreamWriter(System.IO.File.OpenWrite(file.FullName)))
        {
          Array.ForEach(values, v => writer.WriteLine(v));
        }

        Assert.IsTrue(values.SequenceEqual(read1(file)));
        Assert.IsTrue(values.SequenceEqual(read1(file)));

        var lines1 = read1(file);
        Assert.IsTrue(values.SequenceEqual(lines1));
        Assert.IsTrue(values.SequenceEqual(lines1));

        Assert.IsTrue(values.SequenceEqual(read2(file)));
        Assert.IsTrue(values.SequenceEqual(read2(file)));

        var lines2 = read2(file);
        Assert.IsTrue(values.SequenceEqual(lines2));
        Assert.IsTrue(values.SequenceEqual(lines2));
      }
      finally
      {
        file.Delete();
      }
    }
  }
}

#region exports_libsolclient_64.txt
/*
DUMPBIN /EXPORTS libsolclient_64.dll > exports_libsolclient_64.txt

Microsoft (R) COFF/PE Dumper Version 14.00.24210.0
Copyright(C) Microsoft Corporation.All rights reserved.



Dump of file libsolclient_64.dll

File Type: DLL

 Section contains the following exports for libsolclient_64.dll

    00000000 characteristics
    5679CFFE time date stamp Tue Dec 22 16:34:38 2015
        0.00 version
           1 ordinal base
         333 number of functions
         333 number of names

   ordinal hint RVA      name

          1    0 0001A890? _solClient_messageDispatcher_create@@YA? AW4solClient_returnCode@@PEAPEAU_solClient_messageDispatcher@@PEBD @Z
          2    1 000139C0? _solClient_messageDispatcher_destroy@@YA? AW4solClient_returnCode@@PEAPEAU_solClient_messageDispatcher@@@Z
          3    2 00013A80? _solClient_messageDispatcher_processQueueEventsWait@@YA? AW4solClient_returnCode@@PEAU_solClient_messageDispatcher@@H @Z
          4    3 000792D0 ?_solClient_queue_create@@YA? AW4solClient_returnCode@@PEAPEAU_solClient_queue@@@Z
          5    4 000795C0? _solClient_queue_dequeue@@YA? AW4solClient_returnCode@@PEAU_solClient_queue@@PEAU_solClient_event@@H @Z
          6    5 00079B00? _solClient_queue_destroy@@YA? AW4solClient_returnCode@@PEAPEAU_solClient_queue@@@Z
          7    6 000793F0 ?_solClient_queue_enqueue@@YA? AW4solClient_returnCode@@PEAU_solClient_queue@@PEAU_solClient_event@@@Z
          8    7 000798B0? _solClient_queue_flush@@YA? AW4solClient_returnCode@@PEAU_solClient_queue@@@Z
          9    8 00079780 ?_solClient_queue_isEmpty@@YAEPEAU_solClient_queue@@@Z
         10    9 000797D0 ?_solClient_queue_peek@@YA? AW4solClient_returnCode@@PEAU_solClient_queue@@PEAU_solClient_event@@@Z
         11    A 00127770 _solClient_contextPropsDefaultWithCreateThread
         12    B 0002A210 _solClient_context_timerAllocCount
         13    C 00066D80 _solClient_flow_getTopicDispatchStatistic
         14    D 000438F0 _solClient_flow_logState
         15    E 00066C50 _solClient_flow_printDispatchTable
         16    F 00066D00 _solClient_flow_printTopicMatches
         17   10 00065BA0 _solClient_flow_registerForTopicDispatchDestroy
         18   11 00013290 _solClient_getFieldFromList
         19   12 0000D790 _solClient_getNumAllocatedPtrs
         20   13 0014C4E4 _solClient_log_appFilterLevel_g
         21   14 00028C80 _solClient_log_output
         22   15 00028BF0 _solClient_log_output_va_list
         23   16 00080C60 _solClient_messageDispatcher_invokeUserCallback
         24   17 0005E480 _solClient_msgHeaderMap_deleteString
         25   18 0005E1F0 _solClient_msgHeaderMap_getString
         26   19 0005E350 _solClient_msgHeaderMap_setString
         27   1A 00058230 _solClient_msg_decodeFromSmf
         28   1B 0005FCD0 _solClient_msg_encodeToSMF
         29   1C 00056840 _solClient_msg_resetAllStats
         30   1D 0004B2C0 _solClient_msg_setTopicSequenceNumber
         31   1E 00065C70 _solClient_session_confirmTopic
         32   1F 00011860 _solClient_session_forceFailure
         33   20 0001A0F0 _solClient_session_forceKAFailure
         34   21 00066B80 _solClient_session_getTopicDispatchStatistic
         35   22 00011A00 _solClient_session_getTransportCount
         36   23 00010DC0 _solClient_session_logRelPub
         37   24 00066920 _solClient_session_printDispatchTable
         38   25 00066B00 _solClient_session_printTopicMatches
         39   26 00065A00 _solClient_session_registerForSubscribeEvents
         40   27 00065AE0 _solClient_session_registerForTopicDispatchDestroy
         41   28 00069690 _solClient_session_topicSubscribeWithCallback
         42   29 000696E0 _solClient_session_topicUnsubscribeWithCallback
         43   2A 00062AA0 _solClient_subscriptionStorage_freeSrcRoutingMatchArray
         44   2B 0005FE00 _solClient_subscriptionStorage_logStats
         45   2C 00080A20 _solClient_transactedSession_setCommitRequestsToDrop
         46   2D 00080A80 _solClient_transactedSession_setCommitResponsesToDrop
         47   2E 00080BA0 _solClient_transactedSession_setFlowRequestsToDrop
         48   2F 00080C00 _solClient_transactedSession_setFlowResponsesToDrop
         49   30 00080960 _solClient_transactedSession_setOpenRequestsToDrop
         50   31 000809C0 _solClient_transactedSession_setOpenResponsesToDrop
         51   32 00080AE0 _solClient_transactedSession_setRollbackRequestsToDrop
         52   33 00080B40 _solClient_transactedSession_setRollbackResponsesToDrop
         53   34 00029040 _solClient_version_set
         54   35 0000DCF0 solClient_appendUUIDString
         55   36 00010000 solClient_bufInfo_getConsumerId
         56   37 0000FF30 solClient_bufInfo_getConsumerIdCount
         57   38 0006DB00 solClient_cacheSession_cancelCacheRequests
         58   39 0006DED0 solClient_cacheSession_destroy
         59   3A 0006B9C0 solClient_cacheSession_eventToString
         60   3B 0006C1D0 solClient_cacheSession_getApplicationData
         61   3C 0006F540 solClient_cacheSession_sendCacheRequest
         62   3D 0006F590 solClient_cacheSession_sendCacheRequestSequence
         63   3E 0006C160 solClient_cacheSession_setApplicationData
         64   3F 0001AC90 solClient_cleanup
         65   40 0004E640 solClient_container_addBoolean
         66   41 00050120 solClient_container_addByteArray
         67   42 0004FA00 solClient_container_addChar
         68   43 0004FBB0 solClient_container_addContainer
         69   44 00050EC0 solClient_container_addDestination
         70   45 00050980 solClient_container_addDouble
         71   46 00050740 solClient_container_addFloat
         72   47 0004EEC0 solClient_container_addInt16
         73   48 0004F310 solClient_container_addInt32
         74   49 0004F7A0 solClient_container_addInt64
         75   4A 0004EA80 solClient_container_addInt8
         76   4B 0004E430 solClient_container_addNull
         77   4C 00050470 solClient_container_addSmf
         78   4D 00050C00 solClient_container_addString
         79   4E 0004ECA0 solClient_container_addUint16
         80   4F 0004F0E0 solClient_container_addUint32
         81   50 0004F540 solClient_container_addUint64
         82   51 0004E860 solClient_container_addUint8
         83   52 000511C0 solClient_container_addUnknownField
         84   53 0004FF00 solClient_container_addWchar
         85   54 0004DBE0 solClient_container_closeMapStream
         86   55 0004BA40 solClient_container_createMap
         87   56 0004BB60 solClient_container_createStream
         88   57 00054FD0 solClient_container_deleteField
         89   58 000519D0 solClient_container_getBoolean
         90   59 00053ED0 solClient_container_getByteArray
         91   5A 00053D30 solClient_container_getByteArrayPtr
         92   5B 00053920 solClient_container_getChar
         93   5C 000517F0 solClient_container_getContainerPtr
         94   5D 00054B00 solClient_container_getDestination
         95   5E 000545F0 solClient_container_getDouble
         96   5F 00051600 solClient_container_getField
         97   60 00054430 solClient_container_getFloat
         98   61 000528D0 solClient_container_getInt16
         99   62 00052F90 solClient_container_getInt32
        100   63 00053610 solClient_container_getInt64
        101   64 000521D0 solClient_container_getInt8
        102   65 000514A0 solClient_container_getNextField
        103   66 00051900 solClient_container_getNull
        104   67 00051750 solClient_container_getSize
        105   68 00054250 solClient_container_getSmf
        106   69 000540C0 solClient_container_getSmfPtr
        107   6A 00054940 solClient_container_getString
        108   6B 000547B0 solClient_container_getStringPtr
        109   6C 00054CB0 solClient_container_getSubMap
        110   6D 00054E40 solClient_container_getSubStream
        111   6E 00052560 solClient_container_getUint16
        112   6F 00052C40 solClient_container_getUint32
        113   70 000532C0 solClient_container_getUint64
        114   71 00051E50 solClient_container_getUint8
        115   72 00053B30 solClient_container_getWchar
        116   73 00051410 solClient_container_hasNextField
        117   74 0005D370 solClient_container_openMapFromPtr
        118   75 0005D410 solClient_container_openStreamFromPtr
        119   76 0004D880 solClient_container_openSubMap
        120   77 0004D890 solClient_container_openSubStream
        121   78 000558D0 solClient_container_rewind
        122   79 0001B130 solClient_context_create
        123   7A 00016A90 solClient_context_destroy
        124   7B 00027300 solClient_context_processEvents
        125   7C 00026DC0 solClient_context_processEventsWait
        126   7D 0000F570 solClient_context_registerForFdEvents
        127   7E 00029650 solClient_context_startTimer
        128   7F 00029980 solClient_context_stopTimer
        129   80 0002A280 solClient_context_timerTick
        130   81 0000F770 solClient_context_unregisterForFdEvents
        131   82 00010FA0 solClient_createQueueNetworkName
        132   83 00048A40 solClient_datablock_alloc
        133   84 00048C80 solClient_datablock_dup
        134   85 00048AA0 solClient_datablock_free
        135   86 00048CC0 solClient_datablock_getDataPtr
        136   87 00042910 solClient_flow_clearStats
        137   88 00044AA0 solClient_flow_destroy
        138   89 00042E20 solClient_flow_dumpExt
        139   8A 0003CBF0 solClient_flow_eventToString
        140   8B 000441E0 solClient_flow_getApplicationData
        141   8C 000427E0 solClient_flow_getDestination
        142   8D 00043290 solClient_flow_getProperty
        143   8E 00042670 solClient_flow_getRxStat
        144   8F 00042520 solClient_flow_getRxStats
        145   90 000431F0 solClient_flow_getSession
        146   91 000440A0 solClient_flow_getTransactedSession
        147   92 00042CD0 solClient_flow_logFlowInfo
        148   93 000429C0 solClient_flow_logStats
        149   94 00043C90 solClient_flow_receiveMsg
        150   95 00042060 solClient_flow_sendAck
        151   96 00044170 solClient_flow_setApplicationData
        152   97 00040450 solClient_flow_setMaxUnacked
        153   98 000402B0 solClient_flow_start
        154   99 00044CE0 solClient_flow_stop
        155   9A 00069560 solClient_flow_topicSubscribeWithDispatch
        156   9B 00069610 solClient_flow_topicUnsubscribeWithDispatch
        157   9C 0000DA90 solClient_generateUUID
        158   9D 0000DB10 solClient_generateUUIDString
        159   9E 000364E0 solClient_getLastErrorInfo
        160   9F 0000DA30 solClient_initialize
        161   A0 0000D7A0 solClient_initializeExt
        162   A1 00028920 solClient_log_categoryToString
        163   A2 00028900 solClient_log_levelToString
        164   A3 00028960 solClient_log_setCallback
        165   A4 000289A0 solClient_log_setFile
        166   A5 00028DF0 solClient_log_setFilterLevel
        167   A6 00028950 solClient_log_unsetCallback
        168   A7 00048E10 solClient_msg_alloc
        169   A8 0005C5B0 solClient_msg_createBinaryAttachmentMap
        170   A9 0005C8E0 solClient_msg_createBinaryAttachmentStream
        171   AA 0005CC40 solClient_msg_createUserPropertyMap
        172   AB 00048FD0 solClient_msg_decodeFromSmf
        173   AC 0005B960 solClient_msg_deleteApplicationMessageId
        174   AD 0005B670 solClient_msg_deleteApplicationMsgType
        175   AE 0005BF70 solClient_msg_deleteCorrelationId
        176   AF 0005E680 solClient_msg_deleteHttpContentEncoding
        177   B0 0005E5D0 solClient_msg_deleteHttpContentType
        178   B1 0005AE60 solClient_msg_deleteReplyTo
        179   B2 0005B380 solClient_msg_deleteSenderId
        180   B3 0005C260 solClient_msg_deleteSenderTimestamp
        181   B4 0005BC80 solClient_msg_deleteSequenceNumber
        182   B5 0005FCC0 solClient_msg_dump
        183   B6 0005E9F0 solClient_msg_dumpExt
        184   B7 00057F90 solClient_msg_dup
        185   B8 0005D5F0 solClient_msg_encodeToSMF
        186   B9 00048700 solClient_msg_extractDatablock
        187   BA 00057030 solClient_msg_free
        188   BB 0005B720 solClient_msg_getApplicationMessageId
        189   BC 0005B430 solClient_msg_getApplicationMsgType
        190   BD 0005D2E0 solClient_msg_getBinaryAttachmentContainerSize
        191   BE 00058FA0 solClient_msg_getBinaryAttachmentField
        192   BF 00058E20 solClient_msg_getBinaryAttachmentMap
        193   C0 00049610 solClient_msg_getBinaryAttachmentPtr
        194   C1 00058CA0 solClient_msg_getBinaryAttachmentStream
        195   C2 0004D480 solClient_msg_getBinaryAttachmentString
        196   C3 0004B4A0 solClient_msg_getCacheRequestId
        197   C4 0004AD80 solClient_msg_getClassOfService
        198   C5 00049750 solClient_msg_getConsumerId
        199   C6 000496E0 solClient_msg_getConsumerIdCount
        200   C7 0005BD30 solClient_msg_getCorrelationId
        201   C8 000497C0 solClient_msg_getCorrelationTagPtr
        202   C9 0004B000 solClient_msg_getDeliveryMode
        203   CA 00049B00 solClient_msg_getDestination
        204   CB 0004A9E0 solClient_msg_getDestinationTopicSuffix
        205   CC 0005C400 solClient_msg_getExpiration
        206   CD 0005E580 solClient_msg_getHttpContentEncoding
        207   CE 0005E570 solClient_msg_getHttpContentType
        208   CF 0004B140 solClient_msg_getMsgId
        209   D0 0004ACD0 solClient_msg_getRcvTimestamp
        210   D1 0005ABC0 solClient_msg_getReplyTo
        211   D2 00049A30 solClient_msg_getSMFPtr
        212   D3 0005B140 solClient_msg_getSenderId
        213   D4 0005C110 solClient_msg_getSenderTimestamp
        214   D5 0005BA10 solClient_msg_getSequenceNumber
        215   D6 0004C5B0 solClient_msg_getStat
        216   D7 0004AEC0 solClient_msg_getTimeToLive
        217   D8 0004B200 solClient_msg_getTopicSequenceNumber
        218   D9 00049890 solClient_msg_getUserDataPtr
        219   DA 00058AF0 solClient_msg_getUserPropertyMap
        220   DB 00049960 solClient_msg_getXmlPtr
        221   DC 0004C540 solClient_msg_isAckImmediately
        222   DD 0004B3F0 solClient_msg_isCacheMsg
        223   DE 0004B8B0 solClient_msg_isDMQEligible
        224   DF 0004B790 solClient_msg_isDeliverToOne
        225   E0 0004B670 solClient_msg_isDiscardIndication
        226   E1 0004AC60 solClient_msg_isElidingEligible
        227   E2 0004B380 solClient_msg_isRedelivered
        228   E3 0004B550 solClient_msg_isReplyMsg
        229   E4 000570F0 solClient_msg_reset
        230   E5 0004C4B0 solClient_msg_setAckImmediately
        231   E6 0005B870 solClient_msg_setApplicationMessageId
        232   E7 0005B580 solClient_msg_setApplicationMsgType
        233   E8 0004AB40 solClient_msg_setAsReplyMsg
        234   E9 00059840 solClient_msg_setBinaryAttachment
        235   EA 00059E50 solClient_msg_setBinaryAttachmentContainer
        236   EB 0005A1C0 solClient_msg_setBinaryAttachmentContainerPtr
        237   EC 00059150 solClient_msg_setBinaryAttachmentPtr
        238   ED 00059B20 solClient_msg_setBinaryAttachmentString
        239   EE 0004AE20 solClient_msg_setClassOfService
        240   EF 0005BE80 solClient_msg_setCorrelationId
        241   F0 0004A550 solClient_msg_setCorrelationTag
        242   F1 00049FF0 solClient_msg_setCorrelationTagPtr
        243   F2 0004B800 solClient_msg_setDMQEligible
        244   F3 0004B6E0 solClient_msg_setDeliverToOne
        245   F4 0004B0A0 solClient_msg_setDeliveryMode
        246   F5 0004A5D0 solClient_msg_setDestination
        247   F6 0004B5C0 solClient_msg_setDiscardIndication
        248   F7 0004ABD0 solClient_msg_setElidingEligible
        249   F8 0005C310 solClient_msg_setExpiration
        250   F9 0005E5B0 solClient_msg_setHttpContentEncoding
        251   FA 0005E590 solClient_msg_setHttpContentType
        252   FB 00049290 solClient_msg_setMaxPoolMem
        253   FC 00059430 solClient_msg_setQueueNamePtr
        254   FD 0005AD00 solClient_msg_setReplyTo
        255   FE 0005AF10 solClient_msg_setReplyToSuffix
        256   FF 0005B290 solClient_msg_setSenderId
        257  100 0005C020 solClient_msg_setSenderTimestamp
        258  101 0005BB60 solClient_msg_setSequenceNumber
        259  102 0004AF60 solClient_msg_setTimeToLive
        260  103 0004A060 solClient_msg_setTopicPtr
        261  104 0004A450 solClient_msg_setUserData
        262  105 00049F10 solClient_msg_setUserDataPtr
        263  106 0005CF70 solClient_msg_setUserPropertyMap
        264  107 0004A4D0 solClient_msg_setXml
        265  108 00049F80 solClient_msg_setXmlPtr
        266  109 00036570 solClient_resetLastErrorInfo
        267  10A 00006DF0 solClient_returnCodeToString
        268  10B 00007630 solClient_rxStatToString
        269  10C 000108C0 solClient_session_clearStats
        270  10D 0001BC70 solClient_session_connect
        271  10E 000201C0 solClient_session_controlMessageReceiveFd
        272  10F 0003B850 solClient_session_create
        273  110 0006BFD0 solClient_session_createCacheSession
        274  111 00047D40 solClient_session_createFlow
        275  112 000110E0 solClient_session_createTemporaryQueueName
        276  113 00010E50 solClient_session_createTemporaryTopicName
        277  114 00084330 solClient_session_createTransactedSession
        278  115 00016FA0 solClient_session_destroy
        279  116 00017480 solClient_session_disconnect
        280  117 00065630 solClient_session_dteUnsubscribe
        281  118 000197C0 solClient_session_dumpExt
        282  119 0001A0D0 solClient_session_endpointDeprovision
        283  11A 0001A0A0 solClient_session_endpointProvision
        284  11B 00068F40 solClient_session_endpointTopicSubscribe
        285  11C 00068FB0 solClient_session_endpointTopicUnsubscribe
        286  11D 000074D0 solClient_session_eventToString
        287  11E 0003C090 solClient_session_getApplicationData
        288  11F 000121F0 solClient_session_getCapability
        289  120 00012150 solClient_session_getContext
        290  121 0000B8F0 solClient_session_getProperty
        291  122 00010310 solClient_session_getRxStat
        292  123 00010120 solClient_session_getRxStats
        293  124 000106E0 solClient_session_getTxStat
        294  125 000104F0 solClient_session_getTxStats
        295  126 00012A60 solClient_session_isCapable
        296  127 00042F70 solClient_session_logFlowInfo
        297  128 00010BE0 solClient_session_logStats
        298  129 0001A1C0 solClient_session_modifyClientInfo
        299  12A 0003C5D0 solClient_session_modifyProperties
        300  12B 0003C1A0 solClient_session_resume
        301  12C 00018000 solClient_session_send
        302  12D 0005E730 solClient_session_sendMsg
        303  12E 000183F0 solClient_session_sendMultiple
        304  12F 000187E0 solClient_session_sendMultipleMsg
        305  130 00018E20 solClient_session_sendMultipleSmf
        306  131 00011F00 solClient_session_sendReply
        307  132 00011AD0 solClient_session_sendRequest
        308  133 000181D0 solClient_session_sendSmf
        309  134 0003C020 solClient_session_setApplicationData
        310  135 0000FDB0 solClient_session_startAssuredPublishing
        311  136 0003C110 solClient_session_suspend
        312  137 00068E30 solClient_session_topicSubscribe
        313  138 00068E70 solClient_session_topicSubscribeExt
        314  139 00068EB0 solClient_session_topicSubscribeWithDispatch
        315  13A 00069730 solClient_session_topicUnsubscribe
        316  13B 00069770 solClient_session_topicUnsubscribeExt
        317  13C 000697B0 solClient_session_topicUnsubscribeWithDispatch
        318  13D 00064470 solClient_session_validateTopic
        319  13E 00065510 solClient_session_xmlSubscribe
        320  13F 000655A0 solClient_session_xmlUnsubscribe
        321  140 00070AB0 solClient_startPcap
        322  141 00070D50 solClient_stopPcap
        323  142 00006E90 solClient_subCodeToString
        324  143 00083560 solClient_transactedSession_commit
        325  144 00047FC0 solClient_transactedSession_createFlow
        326  145 00083360 solClient_transactedSession_destroy
        327  146 00080880 solClient_transactedSession_getSessionName
        328  147 00083A10 solClient_transactedSession_rollback
        329  148 00080490 solClient_transactedSession_sendMsg
        330  149 00007600 solClient_txStatToString
        331  14A 00084B30 solClient_utils_parseSubscriptionList
        332  14B 00084810 solClient_utils_unescapeString
        333  14C 00028FD0 solClient_version_get


 Summary

       51000 .data
        9000 .pdata
       48000 .rdata
        2000 .reloc
        1000 .rsrc
      B5000 .text
        1000 .tls
*/
  #endregion