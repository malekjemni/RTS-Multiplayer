import mongoose from 'mongoose';
const { model, Schema } = mongoose

const CellModel = new Schema({
    region: {
        type: String,
        required: true,
       
    },
    productivite: {
        type: Number,
        required: true,
       
    },
    level: {
        type: Number,
        required: true,
       
    },
    state: {
        type: Boolean,
        required: true,
       
    },
    index: {
        type: Number,
        required: true,
       
    }

});
export default model('Cell',CellModel);