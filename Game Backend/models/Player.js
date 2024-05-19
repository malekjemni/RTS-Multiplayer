import mongoose from 'mongoose';
const { model, Schema } = mongoose
import Joi from 'joi'
const PlayerModel = new Schema({
    username: {
        type: String,
        required: true,
        unique: true
    },
    password: {
        type: String,
        required: true
    },
    score: {
        type: Number,       
    },
    gold: {
        type: Number,       
    },
    storagewood :{
        type: Number ,
        required :false ,
       
   },
   storagemud :{
       type: Number ,
       required :false ,
     
  },
  storageclay :{
   type: Number ,
   required :false ,
  
},
storageenergie :{
   type: Number ,
   required :false ,
},
MaxWood :{
type: Number ,
  
},
MaxMud :{
   type: Number ,
 
},
MaxClay :{
type: Number ,

},
MaxEnergie :{
type: Number ,
 
},
cell :[{
    type: mongoose.Schema.ObjectId,
    ref : 'cell'
}],
        iron: {
            type: Number,

        },
        wood :{
            type: Number ,
            required :false ,

        },
        mud :{
            type: Number ,
            required :false ,

        },
       solar :{
            type: Number ,
            required :false ,

        },
        wind :{
            type: Number ,
            required :false ,

        },
        water :{
            type: Number ,
            required :false ,

        },
}
);
export function userValidate(player){
    const schema = Joi.object({
        username: Joi.string().min(4).max(30).required(),
        password: Joi.string().min(5).required(),

    });

    return schema.validate(player);
}

export function loginValidate(player){
    const schema = Joi.object({
        username: Joi.string().min(4).required(),
        password: Joi.string().min(5).required()
    });

    return schema.validate(player);
}
export default model('Player',PlayerModel);
