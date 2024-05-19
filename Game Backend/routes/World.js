import express from 'express';
import { body, param } from 'express-validator';
import {
    createWorld,
    getWorldForPlayer,
    addCellToWorld,
    doesPlayerHaveWorld,
    getCellFromWorld
} from '../controllers/World.js'; 

const router = express.Router();

router.route('/world/create')
.post(
    body('player'),
    body('cells'),
    createWorld
)
router.route('/world/addCell')
.post(
    body('worldId'),
    body('cellId'),
    addCellToWorld
)
router.route('/world/:playerId')
.get(getWorldForPlayer)

router.route('/check/:playerId')
.get(doesPlayerHaveWorld)

router.route('/world/:playerId/:cellIndex')
.get(getCellFromWorld)


export default router;