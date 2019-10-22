# [1.0.0](https://github.com/Eastrall/Sylver.Network/releases/tag/v1.0) (2019-10-22)

✨ Initial release

### Features

- **NetServer:** Configuration (Listen address, port, buffer size per clients)
- **NetServer:** Listen for incoming clients
- **NetServer:** Client management
- **NetServer:** Broadcast messages to all connected clients or a given list of connected clients
- **NetClient:** Configuration (Remote address, port, buffer size, retry configuration)
- **NetClient:** Connect to remote server
- **NetClient:** Disconnect from remote server
- **NetClient:** Send/Receive messages to/from a server
- **NetPacket:** Create packet streams
- **NetPacket:** Read data from packet streams
- **NetPacket:** Write data to packet streams
- **NetPacket:** Customize your packet structure with a packet processor