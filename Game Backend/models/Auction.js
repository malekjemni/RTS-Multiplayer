import mongoose from 'mongoose';
const { model, Schema } = mongoose;

const AuctionModel = new Schema({
    seller: {
        type: mongoose.Schema.ObjectId,
        ref: 'Player',
        required: true
    },
    sellerResourceType: {
        type: String,
        required: true,
    },
    sellerAmount: {
        type: Number,
        required: true,
    },
    buyerResourceType: {
        type: String,
        required: true,
    },
    buyerAmount: {
        type: Number,
        required: true,
    },
    state: {
        type: Boolean,
        default: false,
        required: true,
       
    }
});

export default model('Auction', AuctionModel);
