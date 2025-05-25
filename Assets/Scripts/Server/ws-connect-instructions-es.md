# Instrucciones de uso del servidor

## Tabla de contenidos
- [Descripción de Comunicación](#descripción-de-comunicación)
- [Conexión al servidor](#conexión-al-servidor)
- [Envío y recibo de mensajes](#envío-y-recibo-de-mensajes)
- [Función de la sala de espera (Lobby)](#función-de-la-sala-de-espera-lobby)
    - [Acciones enviables](#acciones-enviables)
        - [Jugadores Conectados](#jugadores-conectados)
        - [Mensaje Público](#mensaje-público)
        - [Mensaje Privado](#mensaje-privado)
    - [Acciones por recibir](#acciones-para-recibir)
        - [Conectado](#conectado)
        - [Jugador Conectado](#jugador-conecvtado)
        - [Jugador Desconectado](#jugador-desconectado)
        - [Mensaje Público Recibido](#mensaje-público-recibido)
        - [Mensaje Privado Recibido](#mensaje-privado-recibido)

## Descripción de Comunicación
La comunicación vía WebSockets se realiza mediante el intercambio de mensajes en tiempo real. Cuando el cliente (usted) solicite una acción, esta se envia mediante un texto por la conexión concretada y luego el servidor la procesa. Según el caso, puede recibir una respuesta de confirmación de la acción o no recibir nada.

De la misma forma, cada vez que el servidor envíe una acción al o los clientes, este enviará un mensaje de texto a todos los jugadores pertinentes que se encuentren conectados.

## Conexión al servidor
Para conectarse al servidor de juego, se le entregará una URL a la que su cliente de juego debe conectarse. Esta URL esta formada de la siguiente manera: 

ws://nombre-del-servidor.com/ws

Puede usar algun servicio como [Postman](https://www.postman.com/) o [Insomnia](https://insomnia.rest/) para realizar pruebas a este endpoint.

Por funcionamiento del servidor, el jugador es desconectado automaticamente si luego de 1 minuto no ha enviado ni recibido acciones. Puede enviar un evento aleatorio (como `'ping'`) cada cierto tiempo para evitar ser desconectado por inactividad.

## Envío y recibo de mensajes
La solicitud y recibo de eventos por parte del cliente se realiza mediante mensajes de texto enviados y recibidos por el canal de comunicación. Estos mensajes están codificados en formato [JSON](https://www.json.org/json-es.html), el cuál es un formato de intercambio de datos mediante serialización utilizando clave-valor.  

El formato del objeto JSON que se utiliza para el intercambio de eventos es el mismo que se recibe como mensaje de bienvenida.

```ts
{
  "event": string,
  "data": any,
}
```

- event: Parámetro de tipo `string`. Contiene el evento que se desea enviar o procesar.
- data: Parámetro de tipo variable. Contiene los datos que se le pasarán al evento o que se adjuntan al recibir uno.

Links de interés:
- [Manejo de cadenas de texto JSON en Godot (en inglés)](https://docs.godotengine.org/en/stable/classes/class_json.html)
- [Manejo de cadenas de texto JSON en Unity](https://docs.unity3d.com/es/530/Manual/JSONSerialization.html)


## Función de la sala de espera (Lobby)
Al realizar la conexión, cada jugador recibirá un mensaje con el id que se le asignó automáticamente. Puede guardarlo para su uso posterior en alguna funcionalidad que lo requiera. Dentro de la sala de espera, usted podrá conversar con el resto de jugadores mediante mensajes públicos y privados.

### Acciones enviables
| Nombre             | Evento                 | Descripción                              |
|--------------------|------------------------|------------------------------------------|
|Jugadores Conectados|'get-connected-players' |Obtiene el listado de jugadores conectados|
|Mensaje Público     |'send-public-message'   |Manda un mensaje a la sala pública        |
|Mensaje Privado     |'send-private-message'  |Manda un mensaje privado a un jugador     |

#### Jugadores Conectados

- <b>Evento</b>: `'get-connected-players'`
- <b>Descripción</b>: Obtiene todos los jugadores que se encuentran conectados en ese momento al servidor.
- <b>Parámetros adicionales</b>: No requerido.
- <b>Respuesta</b>: (`string[]`) Listado de identificadores de jugadores.

Esta funcion sirve para obtener los jugadores conectados y así poder enviarles mensajes a cada uno. Puede guardar este listado en un arreglo, representarlo en una interfaz gráfica y utilizar los IDs para mandar mensajes privados.

Ejemplo:
```json
{
  "event": "get-connected-players"
}
```
Ejemplo de respuesta:
```json
{
  "event": "get-connected-players",
  "data": [
    "20d14ac9-4071-45d9-bc8c-b4ad0562ba93"
  ]
}
```

#### Mensaje Público

- <b>Evento</b>: `'send-public-message'`
- <b>Descripción</b>: Envía un mensaje público a la sala pública.
- <b>Parámetros adicionales</b>: 
    - message: (`string`) Mensaje a enviar.
- <b>Respuesta</b>: (`string`) Texto con el estado de la solicitud.

Esta funcion sirve para mandar un mensaje a todos los jugadores conectados al chat grupal. Si el mensaje se envió, recibirá una respuesta de confirmación. De la misma forma, todos los jugadores recibirán el evento `public-message`, indicando que recibieron un mensaje nuevo (véase [Mensaje Público Recibido](#mensaje-público-recibido)).

Ejemplo:
```json
{
  "event": "send-public-message",
  "data": {
    "message": "Holaaaaaa!"
  }
}
```
Ejemplo de respuesta:
```json
{
  "event": "send-public-message",
  "data": "Message sended"
}
```

#### Mensaje Privado

- <b>Evento</b>: `'send-private-message'`
- <b>Descripción</b>: Envía un mensaje privado a un jugador indicado.
- <b>Parámetros adicionales</b>:
    - id: (`string`) ID del jugador conectado.
    - message: (`string`) Mensaje a enviar.
- <b>Respuesta</b>: (`string`) Texto con el estado de la solicitud.

Esta funcion sirve para mandar un mensaje a un jugador específico que se encuentre conectado. Si el mensaje se envió, recibirá una respuesta de confirmación. Acto seguido, el jugador al que le envió el mensaje recibirá el evento `private-message`, indicando que recibió un mensaje nuevo (véase [Mensaje Privado Recibido](#mensaje-privado-recibido)).

Ejemplo de solicitud:
```json
{
  "event": "send-private-message",
  "data": {
    "id": "d830bd74-8bc9-4092-99bf-d88f38ea8006",
    "message": "Holaaaaaa!"
  }
}
```
Ejemplo de respuesta:
```json
{
  "event": "send-private-message",
  "data": "Message sended"
}
```

### Acciones para recibir
Estos eventos son enviados por el servidor al momento que este u otro usuario realizan alguna acción. Su cliente de juego debe escuchar los mensajes que el servidor puede enviar, interpretarlos, y realizar las acciones que sean necesarias para su correcto funcionamiento.

| Nombre                 | Evento              | Descripción                                         |
|------------------------|---------------------|-----------------------------------------------------|
|<b>Acciones del servidor</b>                                                                        |
|Conectado               |'connected-to-server'|Indica que estas conectado al servidor               |
|Jugador conectado       |'player-connected'   |Indica que un jugador se ha conectado                |
|Jugador desconectado    |'player-disconnected'|Indica que un jugador se ha desconectado             |
|<b>Acciones de otros jugadores</b>                                                                  |
|Mensaje Público Recibido|'public-message'     |Indica que ha recibido un mensaje de la sala pública |
|Mensaje Privado Recibido|'private-message'    |Indica que ha recibido un mensaje de otro jugador    |

#### Conectado

- <b>Evento</b>: `'connected-to-server'`
- <b>Descripción</b>: Indica que te has conectado sin problemas al servidor.
- <b>Respuesta</b>:
    - msg: (`string`) Mensaje que indica que te encuentras conectado.
    - id: (`string`) ID que se te ha asignado.

Este evento te indica si lograste conectarte al servidor con éxito. Este es un mensaje meramente informativo para recuperar el identificador que se te asignó. No es necesario para nada más.

Ejemplo de respusta:
```json
{
  "event": "connected-to-server",
  "data": {
    "msg": "You are connected to the game server",
    "id": "d830bd74-8bc9-4092-99bf-d88f38ea8006"
  }
}
```

#### Jugador Conectado

- <b>Evento</b>: `'player-connected'`
- <b>Descripción</b>: Indica que un jugador se ha conectado.
- <b>Respuesta</b>:
    - msg: (`string`) Mensaje que indica que un jugador se conectó.
    - id: (`string`) ID del jugador que se ha conectado.

Este evento le avisa que otro jugador se ha conectado al servidor. El mensaje en cuestión se utiliza para actualizar la lista de jugadores que guarda en su cliente de juego, agregando al jugador conectado. De esta forma, no necesita realizar una llamada al método `'get-connected-players'` para actualizar la lista a cada momento.

Ejemplo de respusta:
```json
{
  "event": "player-connected",
  "data": {
    "msg": "Player d71fe7f8-e3e7-4222-80df-0dae4199c0a4 has connected",
    "id": "d71fe7f8-e3e7-4222-80df-0dae4199c0a4"
  }
}
```

#### Jugador Desconectado

- <b>Evento</b>: `'player-disconnected'`
- <b>Descripción</b>: Indica que un jugador se ha desconectado.
- <b>Respuesta</b>:
    - msg: (`string`) Mensaje que indica que un jugador se desconectó.
    - id: (`string`) ID del jugador que se ha desconectado.

Este evento le avisa que un jugador se ha desconectado del servidor. El mensaje se utiliza para actualizar la lista de jugadores que guarda en su cliente de juego, removiendo al jugador desconectado. De esta forma, no necesita realizar una llamada al método `'get-connected-players'` para actualizar la lista a cada momento.

Ejemplo de respusta:
```json
{
  "event": "player-disconnected",
  "data": {
    "msg": "Player 23a3e557-491f-412e-ab85-c5ed6675ca3c has disconnected",
    "id": "23a3e557-491f-412e-ab85-c5ed6675ca3c"
  }
}
```

#### Mensaje Público Recibido

- <b>Evento</b>: `'public-message'`
- <b>Descripción</b>: Indica que se ha enviado un mensaje a la sala pública.
- <b>Respuesta</b>:
    - id: (`string`) ID del jugador que ha mandado el mensaje.
    - msg: (`string`) Mensaje que el jugador ha mandado a la sala pública.

Este evento le avisa que un jugador ha enviado un mensaje a la sala pública. De esta forma, cada vez que reciba esta acción puede agregar el mensaje a una interfaz gráfica, y asi conversar con el resto de jugadores.

Ejemplo de respusta:
```json
{
  "event": "public-message",
  "data": {
    "id": "0b62b9a1-c51d-4be4-8b76-cdc15b98c82b",
    "msg": "Holaaaaaa!"
  }
}
```

#### Mensaje Privado Recibido

- <b>Evento</b>: `'private-message'`
- <b>Descripción</b>: Indica que ha recibido un mensaje por privado.
- <b>Respuesta</b>:
    - id: (`string`) ID del jugador que le ha mandado el mensaje.
    - message: (`string`) Mensaje que el jugador le ha enviado.

Este evento le avisa que un jugador le ha enviado un mensaje por privado. De esta forma, cada vez que reciba esta acción puede agregar el mensaje a una interfaz gráfica, y asi conversar con el jugador en cuestión.

Ejemplo de respusta:
```json
{
  "event": "send-private-message",
  "data": {
    "id": "d830bd74-8bc9-4092-99bf-d88f38ea8006",
    "message": "Holaaaaaa!"
  }
}
```
