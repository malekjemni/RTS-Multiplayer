import RoomModel from '../models/Room.js'; 

// Function to create a room
export async function createRoom(req, res) {
    try {
        const {state} = req.body;
        const rewRoom = new RoomModel({
            state,
        });

        const savedRoom = await rewRoom.save();
        res.status(201).json(savedRoom);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while creating the room.' });
    }
}

// Function to add a player to a room
export async function addPlayerToRoom(req, res) {
    try {
        const {roomId,playerId} = req.body;
        const room = await RoomModel.findById(roomId);
        if (!room) {
            throw new Error('Room not found');
        }
        room.players.push(playerId);
        const savedRoom = await room.save();
        res.status(201).json(savedRoom);
    } catch (error) {
        console.error('Error adding player to room:', error);
        throw error;
    }
}

// Function to delete a room
export async function deleteRoom(roomId) {
    try {
        const deletedRoom = await RoomModel.findByIdAndDelete(roomId);
        return deletedRoom;
    } catch (error) {
        console.error('Error deleting room:', error);
        throw error;
    }
}

// Function to update a room
export async function updateRoom(roomId, updatedRoomData) {
    try {
        const updatedRoom = await RoomModel.findByIdAndUpdate(roomId, updatedRoomData, { new: true });
        return updatedRoom;
    } catch (error) {
        console.error('Error updating room:', error);
        throw error;
    }
}

// Function to get players in a room
export async function getPlayersInRoom(req, res) {
    try {
        const { roomId } = req.params;
        const room = await RoomModel.findById(roomId).populate('players');
        if (!room) {
            throw new Error('Room not found');
        }
        return res.status(200).json(room.players);      
    } catch (error) {
        console.error('Error getting players in room:', error);
        throw error;
    }
}

export async function getAllRooms(req, res) {
    try {
        const rooms = await RoomModel.find();

        res.status(200).json(rooms);
    } catch (error) {
        console.error('Error retrieving rooms:', error);
        res.status(500).json({ error: 'An error occurred while retrieving rooms.' });
    }
}
export async function deleteRooms(req, res) {
    try {
        const deletedResult = await RoomModel.deleteMany({});
        res.status(200).json({ message: 'All rooms deleted successfully.', deletedCount: deletedResult.deletedCount });
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while deleting the rooms.' });
    }
}