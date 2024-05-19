import WorldModel from '../models/World.js';
import CellModel from '../models/Cell.js';

export async function createWorld(req, res) {
    try {
        const { player, cells } = req.body;
          const world = new WorldModel({
            player,
            cells,
          });
        const newWorld = await world.save();
        res.status(201).json(newWorld);
    } catch (error) {
        console.error('Error creating world:', error);
        res.status(500).json({ error: 'An error occurred while creating the world.' });
    }
}

export async function getWorldForPlayer(req, res) {
    try {
        const { playerId } = req.params;
        const existingWorld = await WorldModel.findOne({ player: playerId }).populate('cells');

        if (existingWorld) {
            res.status(200).json({cells : existingWorld.cells});
        } else {
            res.status(404).json({ error: "player don't have a world" });
        }
    } catch (error) {   
        console.error('Error getting world for player:', error);
        res.status(500).json({ error: 'An error occurred while getting world for player' });
    }
}

export async function addCellToWorld(req, res) {
    try {
        const {worldId,cellId } = req.body;
        const world = await WorldModel.findById(worldId);
        if (!world) {
            throw new Error('world not found');
        }
        world.cells.push(cellId);
        const savedWorld = await world.save();
        res.status(201).json(savedWorld);
    } catch (error) {
        console.error('Error adding cell to world:', error);
        throw error;
    }
}

export async function doesPlayerHaveWorld(req, res) {
    try {
        const { playerId } = req.params;
        const existingWorld = await WorldModel.findOne({ player: playerId });
        res.status(200).json(!!existingWorld);
    } catch (error) {
        console.error('Error checking if player has a world:', error);
        res.status(500).json({ error: 'An error occurred  checking player' });
    }
}

export async function getCellFromWorld(req, res) {
    try {
        const { playerId, cellIndex } = req.params;

        const world = await WorldModel.findOne({ player: playerId });

        if (!world) {
            return res.status(404).json({ error: 'World not found for the player.' });
        }

        for (const cellId of world.cells) {
            const cell = await CellModel.findById(cellId);

            if (cell && cell.index === parseInt(cellIndex, 10)) {
                return res.status(200).json(cell);
            }
        }

            return res.status(404).json({ error: 'Cell not found.' });
     } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while retrieving the cell data.' });
    }
}