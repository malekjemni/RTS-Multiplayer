import { validationResult } from "express-validator";

import Player, { loginValidate, userValidate }  from '../models/Player.js'
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken'
import nodemailer from 'nodemailer';
import { Error, model } from 'mongoose';
//import path from 'path';
import { fileURLToPath } from 'url';
import { render } from 'ejs';
//import sendConfirmationEmail from "../middlewares/mailer.js";

import moment from 'moment';
import schedule from 'node-schedule';
import dotenv from 'dotenv';





dotenv.config();
export async function addPlayer(req, res) {
    if (!validationResult(req).isEmpty()) {
        res.status(400).json({ errors: validationResult(req).array() });
    }
    else {
        Player.findOne({ $or: [ { username: req.body.username }] })
            .then(async (existingPlayer) => {
                if (existingPlayer) {
                    res.status(409).json({error: "Player with the same username already exists"});
                } else {
                    const hashedPassword = await bcrypt.hash(req.body.password, 10)
                    const { username} = req.body
                    Player.create(
                        {
                            username: req.body.username,
                            password: hashedPassword,
                            score: 0,
                            gold: 1000,
                            storagewood: 0,
                            storagemud: 0,
                            storageclay: 0,
                            storageenergie: 0,
                            MaxWood: 25000,
                            MaxClay: 20000,
                            MaxMud: 30000,
                            MaxEnergie: 10000,
                            iron: 0,
                            wood: 0,
                            mud: 0,
                            solar: 0,
                            wind: 0,
                            water: 0,

                        }
                    )
                        .then((newPlayer) => {
                            res.status(201).json(newPlayer);
                        })
                        .catch((err) => {
                            res.status(500).json({error: err});
                        });
                }
            })
            .catch((err) => {
                res.status(500).json({ error: err });
            });
    }
}

export async function login(req, res) {
    try {
        if (!validationResult(req).isEmpty()) {
            res.status(400).json({ errors: validationResult(req).array() });
        }

        const { username, password } = req.body;
        const player = await Player.findOne({ username });


        if (!player) {
            return res.status(404).send({error: "Invalid username!"});
        }

        const checkPassword = await bcrypt.compare(password, player.password);

        if (!checkPassword) {
            return res.status(404).send({error: "Invalid password!"});
        }

        // Generate a JSON Web Token (JWT) for user authentication
        console.log(process.env.JWT_SECRET); // Add this for debugging
        const token = jwt.sign({ username: player.username }, 'mysecret', {
            expiresIn: '1h',
        });


        res.status(200).json({ token , player});
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: err.message });
    }
}


export async function getAll(req, res) {
    try {
        const players = await Player.find({}).lean();
        console.log(players);
        res.status(200).json({ players }); 
    }
    catch (err) {
        res.status(500).json({ message: err.message });
    }
}

export function getPlayer(req, res) {
    Player.findOne({ username: req.body.username })
        .then((player) => {
            if (!player) {
                res.status(404).json({ error: "player not found" });
            } else {
                res.status(200).json(player);
            }
        })
        .catch((err) => {
            res.status(500).json({ error: err });
        });
}
export function updatePlayer(req, res) {
    //console.log(req.body);
    // Assuming you want to find the player by username and update other fields
    const { username } = req.params;
    // Create an object for the update operation
    let updateData = {};

    // Check each field for existence before adding it to the updateData object
    if (req.body.username !== undefined) updateData.username = req.body.username;
    if (req.body.password !== undefined) updateData.password = req.body.password;
    if (req.body.score !== undefined) updateData.score = req.body.score;
    if (req.body.gold !== undefined) updateData.gold = req.body.gold;
    if (req.body.storagewood !== undefined) updateData.storagewood = req.body.storagewood;
    if (req.body.storagemud !== undefined) updateData.storagemud = req.body.storagemud;
    if (req.body.storageclay !== undefined) updateData.storageclay = req.body.storageclay;
    if (req.body.storageenergie !== undefined) updateData.storageenergie = req.body.storageenergie;

    if (req.body.MaxWood !== undefined) updateData.MaxWood = req.body.MaxWood;
    if (req.body.MaxClay !== undefined) updateData.MaxClay = req.body.MaxClay;
    if (req.body.MaxMud !== undefined) updateData.MaxMud = req.body.MaxMud;
    if (req.body.MaxEnergie !== undefined) updateData.MaxEnergie = req.body.MaxEnergie;

    if (req.body.iron !== undefined) updateData.iron = req.body.iron;
    if (req.body.wood !== undefined) updateData.wood = req.body.wood;
    if (req.body.mud !== undefined) updateData.mud = req.body.mud;
    if (req.body.solar !== undefined) updateData.solar = req.body.solar;
    if (req.body.wind !== undefined) updateData.wind = req.body.wind;
    if (req.body.water !== undefined) updateData.water = req.body.water;

    // Now update the player with the constructed updateData object
    Player.updateOne({ username: username }, { $set: updateData })
        .then(updateResponse => {
            console.log(updateResponse); // This will show you the structure of the response object.
            //res.status(200).json(updateData);
            // Check the correct properties from the response object
            // The actual properties might be `n` and `nModified` depending on your Mongoose version
            if (updateResponse.n === 0) {
                res.status(404).json({ message: "Player not found." });
            } else if (updateResponse.nModified === 0) {
                res.status(200).json({ message: "No changes were made to the player." });
            } else {
                res.status(200).json(updateData);
            }
        })
        .catch(err => {
            res.status(500).json({ error: err.message });
        });

}


export async function getOnce(req, res) {

    await Player
        .findById(req.params.id)
        .then(docs => {
            res.status(200).json(docs);
        })
        .catch(err => {
            res.status(500).json({ error: err });
        });
}
export async function getUserbyUserName(req, res) {
    try {
        let username = req.params.UserName; // Make sure this matches the route parameter

        const user = await Player.findOne({ username: username }) // Removed unnecessary .populate() calls

        if (!user) {
            return res.status(404).json({ message: `User with username ${username} not found` });
        }

        res.status(200).json(user);
    } catch (error) {
        res.status(500).json({ message: error.message });
    }
}

export async function getPlayerName(req, res) {
    try {
        const { playerId } = req.params;
        const player = await Player.findById(playerId);
        if (!player) {
            throw new Error('player not found');
        }
        return res.status(200).json(player.username);      
    } catch (error) {
        console.error('Error :', error);
        throw error;
    }
}

export async function getLeaderboard(req, res) {
    try {
        const players = await Player.find().sort({ score: -1 });
        if (!players) {
            return res.status(404).json({ error: 'No players found.' });
        }
        res.status(200).json({players});
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'An error occurred while retrieving the leaderboard.' });
    }
}