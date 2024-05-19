import express from 'express';
import { body, param } from 'express-validator';
import {
    createRoom,
    addPlayerToRoom,
    getPlayersInRoom,
    deleteRooms,
    getAllRooms,
    updateRoom
} from '../controllers/Room.js';

const router = express.Router();

router.route('/room/create')
.post(
    body('state').optional().isIn(['lobby', 'playing', 'finished']),
    createRoom
)
router.route('/room/addPlayer')
.post(
    body('roomId'),
    body('playerId'),
    addPlayerToRoom
)
router.get('/room/:roomId', getPlayersInRoom)
router.get('/rooms', getAllRooms)
router.delete('/rooms/clear',deleteRooms)
export default router;
