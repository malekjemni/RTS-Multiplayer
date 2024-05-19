import mongoose from 'mongoose';

const { Schema, model } = mongoose;

const RoomModel = new Schema({
    state: {
        type: String,
        enum: ['lobby', 'playing', 'finished'],
        default: 'lobby'
    },
    players: [{ type: mongoose.Schema.Types.ObjectId, ref: 'Player' }],
});


export default model('Room', RoomModel);