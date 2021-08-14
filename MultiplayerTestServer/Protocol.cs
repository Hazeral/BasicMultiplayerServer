namespace MultiplayerTestServer
{
    public enum PacketType
    {
        PlayerWelcome,
        PlayerJoin,
        PositionsUpdate,
        PlayerInput,
        PlayerDisconnect,
        Ping,
        ChatMessage,
        ServerMessage,
        ClearMessages
    }

    /*
    
        Packets begin with a packet ID followed by a $ to separate the payload, the packet is also suffixed with a ~ to identify the end of a packet
        Most payloads for sharing player information will be formatted similar to id:info (i.e. id:x,y)

        Example:
            1$sd8c:-3,-7~
            1 refers to the PlayerJoin packet
            The payload consists of the new player's identifier followed by current coordinates


        Some other packets such as PositionsUpdate which contain information for multiple players are formatted in a similar fashion but each player's information is separated by a ;
    
        Example:
            2$sd8c:-3,-7;8wsf:5,0;xcu8:-5,2~
            Each player's information in the payload is separated by a ; (sd8c:-3,-7  8wsf:5,0  xcu8:-5,2)


        Finally, there are packets which will contain little to no payload, the PlayerDisconnect packet does not require coordinates and will simply send the ID in the payload
        Whereas the ping payload will simply send an empty payload as it is used to only identify whether the client is still connected

        Example:
            5$~
            The payload is empty but the packet will still contain the payload seperator ($) 


        The initial packet sent (PlayerWelcome) will be unencrypted, the packets which follow will then use XOR encryption with the player ID as the key

    */
}
