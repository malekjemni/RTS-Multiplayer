import express from 'express';
import { body, param } from 'express-validator';
import {
    createCell,
    updateCell,
    deleteCell,
    getCell,
    getAllCells,
    deleteAllCells
} from '../controllers/Cell.js'; 

const router = express.Router();

router.route('/cell/create')
.post(
        body('region').isString().notEmpty().withMessage('Region is required.'),
        body('productivite').isNumeric().withMessage('Productivity must be a number.'),
        body('level').isNumeric().withMessage('Level must be a number.'),
        body('state').isBoolean().withMessage('State must be a boolean.'),
        body('index').isNumeric().withMessage('index must be a number.'),
    createCell
)

router.route('/cell/:cellId')
.put(
        body('region').optional().isString(),
        body('productivite').optional().isNumeric(),
        body('level').optional().isNumeric(),
        body('state').optional().isBoolean(),
    updateCell
)

router.route('/cell/:cellId')
.delete(
        param('cellId').isMongoId().withMessage('Cell ID must be a valid MongoDB ObjectId.'),
    deleteCell
)
router.get('/cell/:index', getCell)
router.get('/cells', getAllCells)
router.delete('/clear',deleteAllCells)

export default router;
