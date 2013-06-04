class client
    {
        Socket m_socClient;

        public void connect(object sender, System.EventArgs e, String ipAddress, String port)
        {
            try
            {
                //create a new client socket
                m_socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szIPSelected = ipAddress;
                String szPort = port;
                int alPort = System.Convert.ToInt16(szPort, 10);

                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                m_socClient.Connect(remoteEndPoint);
                String szData = "Connection Established, Kinect Ready!";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                m_socClient.Send(byData);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }


        public void send(object sender, System.EventArgs e, Object data)
        {
            try
            {
                Object objData = data;
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                m_socClient.Send(byData);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }


        public void close(object sender, System.EventArgs e)
        {
            m_socClient.Close();
        }

    }