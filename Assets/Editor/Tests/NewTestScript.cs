using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        [Test]
        public void TestToJson()
        {
            String json = "{\"code\":203,\"message\":\"success\",\"data\":[{\"point\":3,\"color\":2},{\"point\":3,\"color\":1},{\"point\":15,\"color\":2},{\"point\":9,\"color\":1},{\"point\":14,\"color\":4},{\"point\":14,\"color\":3},{\"point\":11,\"color\":3},{\"point\":10,\"color\":2},{\"point\":14,\"color\":1},{\"point\":8,\"color\":4},{\"point\":7,\"color\":2},{\"point\":11,\"color\":2},{\"point\":9,\"color\":4}]}";
            
        }

        [Test]
        public void TestJson()
        {
            // var json =
            //     "{\"code\":200,\"message\":\"success\",\"data\":[{\"name\":\"Hall\",\"roomNumber\":0,\"playerCount\":1,\"playerNameList\":[\"Fky\"]},{\"name\":\"Hall2\",\"roomNumber\":1,\"playerCount\":1,\"playerNameList\":[\"Fky\"]},{\"name\":\"Hall3\",\"roomNumber\":2,\"playerCount\":1,\"playerNameList\":[\"Fky\"]},{\"name\":\"Hall4\",\"roomNumber\":3,\"playerCount\":1,\"playerNameList\":[\"Fky\"]}]}";

            var json =
                "{\"code\":202,\"message\":\"success\",\"data\":{\"name\":\"Hall\",\"roomNumber\":0,\"playerCount\":1,\"playerNameList\":[\"Fky\"]}}";
            var commonResponse = JsonUtility.FromJson<WebSocketManeger.CommonResponse<String>>(json);
            WebSocketManeger.Room room;
            List<WebSocketManeger.Room> roomList;

            switch (commonResponse.code)
            {
                case 200:
                    break;
                case 201:
                    roomList = JsonUtility.FromJson<WebSocketManeger.CommonResponse<List<WebSocketManeger.Room>>>(json)
                        .data;
                    Assert.True(roomList != null);
                    break;
                case 202:
                    room = JsonUtility.FromJson<WebSocketManeger.CommonResponse<WebSocketManeger.Room>>(json).data;
                    Assert.True(room != null);
                    break;
            }
        }

        // A Test behaves as an ordinary method
        [Test]
        public void TestGroupByCount()
        {
            Assert.True(TestPokerController.Color.方块 < TestPokerController.Color.黑桃);
            Assert.True(TestPokerController.Color.方块 < TestPokerController.Color.梅花);
            Assert.True(TestPokerController.Color.方块 < TestPokerController.Color.红桃);

            Assert.True(TestPokerController.Color.梅花 < TestPokerController.Color.红桃);
            Assert.True(TestPokerController.Color.梅花 < TestPokerController.Color.黑桃);

            Assert.True(TestPokerController.Color.红桃 < TestPokerController.Color.黑桃);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [Test]
        public void TestWebSocket()
        {
            Connect();
        }

        async void Connect()
        {
            var clientWebSocket = new ClientWebSocket();
            try
            {
                await
                    clientWebSocket.ConnectAsync(new Uri("localhost:8080/topic/greetings"), CancellationToken.None);


                if (clientWebSocket.State == WebSocketState.Open)
                    Console.WriteLine("open");
                else
                {
                    Console.WriteLine("Not open");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }


    public class TestPokerController
    {
        public int point;

        public TestPokerController(int point)
        {
            this.point = point;
        }

        public class Poker
        {
            public int point;
            public Color color;
        }

        public enum Color
        {
            黑桃 = 4,
            红桃 = 3,
            梅花 = 2,
            方块 = 1
        }
    }
}