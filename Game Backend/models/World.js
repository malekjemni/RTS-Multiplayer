import mongoose from 'mongoose';
const { model, Schema } = mongoose;

const WorldModel = new Schema({
    player: {
        type: mongoose.Schema.ObjectId,
        ref: 'Player',
        required: true
    },
    cells: [{
        type: mongoose.Schema.ObjectId,
        ref: 'Cell'
    }]
});

export default model('World', WorldModel);
