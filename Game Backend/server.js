import express from 'express';
import mongoose from 'mongoose';
import morgan from 'morgan';
import cors from 'cors';
import * as dotenv from "dotenv";
import http from 'http';
import WebSocket from 'ws';
import { WebSocketServer } from 'ws';

const app = express();
const port = process.env.PORT || 9090;
const databaseName = 'empireBD';
const db_url = process.env.DB_URL || `mongodb://127.0.0.1:27017`;

import routesPlayer from './routes/Player.js'
import routesCell from './routes/Cell.js'
import routersRoom from  './routes/Room.js'
import routersWorld from  './routes/World.js'
import routersAuction from  './routes/Auction.js'

mongoose.set('debug', true);
mongoose.Promise = global.Promise;


const server = http.createServer(app);

const wss = new WebSocketServer({server});

let timerValue = 0; 
let countDownTimerValue = 60;
const initialCountDownValue = 60;  
let countDownTimerInterval;

function broadcastTimerUpdate() {
    if (timerValue <= 60 * 60) {
        wss.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(JSON.stringify({ type: 'timerUpdate', value: timerValue }));
            }
        });
        timerValue++;
    } else {
        clearInterval(timerInterval);
    }
}

const timerInterval = setInterval(broadcastTimerUpdate, 1000);



function startCountDownTimer() {
    countDownTimerValue = initialCountDownValue; 
    countDownTimerInterval = setInterval(broadcasCountDownTimerUpdate, 1000); 
}

function stopCountDownTimer() {
    clearInterval(countDownTimerInterval); 
}

function broadcasCountDownTimerUpdate() {
    if (countDownTimerValue >= 0) {
        wss.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(JSON.stringify({ type: 'countDownTimerUpdate', value: countDownTimerValue }));
            }
        });
        countDownTimerValue--; 
    } else {
        clearInterval(countDownTimerInterval);
    }
}

wss.on('connection', (ws) => {
    console.log('WebSocket client connected');
    ws.on('close', () => {
        console.log('WebSocket client disconnected');
    });
});

mongoose
  .connect(`${db_url}/${databaseName}`)
  .then(() => {
    console.log(`Connected to ${databaseName}`);
  })
  .catch(err => {
    console.log(err);
  });

app.use(cors({
    origin: 'http://127.0.0.1:9090',
    methods: ['GET', 'POST'],
    credentials: true,
}));
app.use(morgan('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
dotenv.config();

// Your existing routes
app.use(routesPlayer); 
app.use(routesCell); 
app.use(routersRoom); 
app.use(routersWorld); 
app.use(routersAuction); 


app.get('/startCountDownTimer', (req, res) => {
  startCountDownTimer();
  res.send('Countdown timer started');
});

app.get('/stopCountDownTimer', (req, res) => {
  stopCountDownTimer(); 
  res.send('Countdown timer stopped');
});


server.listen(port, () => {
    console.log(`Server running at http://localhost:${port}/`);
});
