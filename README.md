# Magic Maze Online

This is a Unity project made for a class on Collaborative Softwares. The focus of this project was to handle multiple interactions in real time.

## Choice

We chose to implement an online version of the boardgame `Magic Maze` for this assignment.

We chose this game due to his very interesting way of collaborating between players. Since it’s a cooperating game, communication is the prime key to win, so restricting basic ways of communicating make players find other methods to give information to others. Furthermore, the fact that players all play at the same time seemed like a good challenge to make an interesting collaboration game.

We chose to make Magic Maze Online on Unity. Since most of us already made some projects with the software and feel comfortable with it, it seemed like we could mainly focus on the collaborative part of the game, which we didn’t know yet.

## Tools used

The library we used for networking is Unity's **Netcode for GameObjects**. It's a high-level networking library developed by Unity. It gives a good abstraction of networking logic and lets us focus directly on building the game without entering into low-level protocols. `Netcode` is designed so the Network interactions are Server-Authoritative. This means the server handles all the internal logic and informs the clients of any change. When clients want to make changes, they have to inform the server beforehand, then the server dispatches them.

In the way Netcode is designed, there is a possibility for clients to rule the logic with Client RPCs (Remote Procedure Calls) but it is in fact discouraged, the server should handle everything that is common. Thus, any Client RPC is called by the server, sometimes after the server receives a Server RPC, and is used only to locally apply changes that were made on the server.

In Netcode, there are 4 main elements that we have to use:
- The `NetworkManager` script which handles all the low-level server logic, with the use of sockets and everything. The only thing left to do on that part is choosing between being a client, a server or a host (both client and server, which is what we use).
- The `NetworkVariable` class which turns member fields into fields synchronized on the network.
- The `NetworkObject` script. It enables a Unity `GameObject` to have a relationship with the NetworkManager and synchronize properly its different variables if they were stated as `NetworkVariables` . 
- The `NetworkBehaviour` base class which is a subclass of the MonoBehaviour class from Unity. It is used to enable interactions with the NetworkManager and the `NetworkVariables` inside of a script. It also enables the creation of Client and Server RPCs in the class. It is to be used on any network-dependent or at least related script.

## Implemented feature

This is but a prototype, so there are bugs and such anyway. We only had to focus on the main interaction and the concurrency handling.

### Game rules

A first tile is created when the game starts.
When a pawn is going on a square at the edge of a tile, a new tile is revealed and put on the map
Pawn can be moved in all directions, depending on the player’s ability card

### Network interactions

- At the start of the game, each player receives a specific ability card, defining which moves they can make.

- When clicking on a pawn to select it, only the player sees which moves are playable with this pawn.

- A pawn can only be selected by one player at time, and when selected, only the player who has selected it sees which moves are playable.


The network model we used is a centralized network, so all communication made is between the server and the clients. When a client wants to move a pawn, all calculations to verify if the move is valid are made on the client side, then if the move is effectively valid, it sends the move request to the server. The server updates the pawn’s new position then transmits it to all clients. The server is not responsible for any calculations or decision making except shuffling the cards and the abilities of each player at the beginning of the game. Otherwise, all calculations are done locally on the player's computer, they share only the tiles that are positioned and the position of the pawns. We did not implement the “signal a person” button but that feature would also be calculated on the client side and shared through the network.

Here, the `Server` is also a `Client`, which qualifies as a `Host`.

## Demo

Click on the image if you want to see a demo run of the game.


[![Demo of the game](https://img.youtube.com/vi/Kr-CqmqWyok/maxresdefault.jpg)](https://youtu.be/Kr-CqmqWyok)
