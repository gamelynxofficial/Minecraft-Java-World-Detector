using Monitoring;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameLynx.MultiplayerAPI.Java {
    internal class McJavaLanGameFinder {
        private readonly UdpClient socket; private readonly IPEndPoint pingGroup;
        public McJavaLanGameFinder() { socket = new UdpClient(4445);
            pingGroup = new IPEndPoint(IPAddress.Parse("224.0.2.60"), 4445);
            socket.JoinMulticastGroup(pingGroup.Address); }
        public async Task<string[]> SearchAsync(CancellationToken token) {
            string locapIp = AddServer.get_adress_local();
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            using (token.Register(() => socket.Close())) { 
                   UdpReceiveResult result = await socket.ReceiveAsync();
                    byte[] receivedBytes = result.Buffer;
                    string LANpayload = Encoding.UTF8.GetString(receivedBytes);
                    if (LANpayload.Contains("[MOTD]") && LANpayload.Contains("[AD]") && 
                    result.RemoteEndPoint.Address.ToString() == locapIp) {
                        var dic = parsePayload(LANpayload); var s = new string[2];
         s[0] = dic["motd"]; s[1] = dic["port_ipv4"]; return s; }  return null; }}
        public Dictionary<string, string> parsePayload(string payload) {
            var dic = new Dictionary<string, string>(); int start = payload.IndexOf("[MOTD]"); int end = payload.IndexOf("[/MOTD]", start + "[MOTD]".Length);
            string motd = payload.Substring(start + "[MOTD]".Length, end - start - "[MOTD]".Length);
            dic.Add("motd", motd); int adStart = payload.IndexOf("[AD]", start + "[/MOTD]".Length); int adEnd = payload.IndexOf("[/AD]", adStart + "[AD]".Length);
            string port = payload.Substring(adStart + "[AD]".Length, adEnd - adStart - "[AD]".Length); dic.Add("port_ipv4", port); return dic;}}}