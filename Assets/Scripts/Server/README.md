# UCN WebSocket Game Server
This is a WebSocket Server to use in the implementation of the Cross-Game Multiplayer Gameplay for "Advanced Programming Integrator Project" Course.

## Description
This server is written in Typescript using NestJS Framework. Support Websocket native connection (using ws library).

## How to Connect?
To use the game server services, you can find the instructions in [This Guide (En español)](./ws-connect-instructions-es.md).

To use the table score functions, you can follow the instructions in [This Guide (En español)](./score-table-connect-instructions-es.md).

## Installation

```bash
$ npm install
```

## Running the app

```bash
# development
$ npm run start

# watch mode
$ npm run start:dev

# production mode
$ npm run start:prod
```

## Test

```bash
# unit tests
$ npm run test

# e2e tests
$ npm run test:e2e

# test coverage
$ npm run test:cov
```

## Support

Nest is an MIT-licensed open source project. It can grow thanks to the sponsors and support by the amazing backers. If you'd like to join them, please [read more here](https://docs.nestjs.com/support).

## Stay in touch

- Author - [Kamil Myśliwiec](https://kamilmysliwiec.com)
- Website - [https://nestjs.com](https://nestjs.com/)
- Twitter - [@nestframework](https://twitter.com/nestframework)

## License

Nest is [MIT licensed](LICENSE).
