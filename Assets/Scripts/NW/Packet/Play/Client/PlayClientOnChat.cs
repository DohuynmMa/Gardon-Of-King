using System;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientOnChat : IPacket
    {
        public string message;
        public string name;
        public PlayClientOnChat(string name, string message)
        {
            this.name = name;
            this.message = message;
        }
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                var cm = ConsoleManager.Instance;
                if (message == "") return;
                var currentMessage = name + "说: " + message;
                if (currentMessage == "") return;
                var m = ConsoleManager.Instantiate(cm.showingMessagePrefab);
                var parent = cm.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0);
                DialogConsole.Instance.showingMessages.SetActive(!TestManager.Instance.隐藏聊天信息);
                m.transform.SetParent(parent);
                cm.sentMessageList.Add(message);
                m.text.text = currentMessage;
                cm.messages.Add(currentMessage); cm.showingMessages.Add(m);
                m.waitToDes(10);
                cm.updateShowingMessagePos();
            });
        }
    }
}
