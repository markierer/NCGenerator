module Shape = 

    type Point = { X: float; Z: float }
    let degrees radiant = radiant * 180.0 / System.Math.PI

    // Start point
    let startPoint = 
        { X = 30.0 
          Z = 0.0 }

    // Left point
    let leftPoint = 
        { X = 50.0 
          Z = 10.0 }

    // Angle alpha
    let alpha = atan2 (leftPoint.X - startPoint.X) (leftPoint.Z - startPoint.Z)
    let alphaDegrees = degrees alpha

    // Right point
    let rightPoint = 
        { X = 60.0 
          Z = 60.0 }

    let beta = atan2 (rightPoint.X - leftPoint.X) (rightPoint.Z - leftPoint.Z)
    let betaDegrees = degrees beta

    // End point
    let endPoint = 
        { X = 30.0 
          Z = 70.0 }

    let gamma = atan2 (endPoint.X - rightPoint.X) (endPoint.Z - rightPoint.Z)
    let gammaDegrees = degrees gamma


module Trajectory = 

    type Point = { X: float; Z: float }
    
    // Cutter diameter
    let cutterDiameter = 20.0
    let cutterRadius = cutterDiameter / 2.0

    // Normal point
    let normalPoint = 
        { X = Shape.startPoint.X + cutterRadius * cos Shape.alpha 
          Z = Shape.startPoint.Z - cutterRadius * sin Shape.alpha }

    // Normal plunge in point
    let plungeInX = 8.94
    let normalPlungeInPoint = 
        { X = normalPoint.X + plungeInX 
          Z = normalPoint.Z - plungeInX * tan Shape.alpha }

    // Tangential point
    let tangentialPoint = 
        { X = normalPoint.X - (cutterRadius - cutterRadius * sin Shape.alpha) * tan Shape.alpha
          Z = normalPoint.Z - (cutterRadius - cutterRadius * sin Shape.alpha) }

    // Tangential point with infeed Z
    let infeedZ = 5.0
    let tangentialInfeedPoint = 
        { X = tangentialPoint.X - infeedZ * tan Shape.alpha
          Z = tangentialPoint.Z - infeedZ }

    // Entry end point
    let entryEndPoint = 
        { X = 
            if (Shape.beta <= 0) then normalPoint.X + (Shape.leftPoint.X - Shape.startPoint.X) + (cutterRadius - cutterRadius * cos Shape.alpha)
            else                      normalPoint.X + (Shape.leftPoint.X - Shape.startPoint.X) + ((cutterRadius - cutterRadius * cos Shape.alpha) - (cutterRadius - cutterRadius * cos Shape.beta))
          Z = 
            if (Shape.beta <= 0) then normalPoint.Z + (Shape.leftPoint.Z - Shape.startPoint.Z) + (cutterRadius - cutterRadius * cos Shape.alpha) / tan Shape.alpha 
            else                      normalPoint.Z + (Shape.leftPoint.Z - Shape.startPoint.Z) + ((cutterRadius - cutterRadius * cos Shape.alpha) - (cutterRadius - cutterRadius * cos Shape.beta)) / tan Shape.alpha }

    // Gear start point
    let gearStartPoint = 
        { X = Shape.startPoint.X + (Shape.leftPoint.X - Shape.startPoint.X) + cutterRadius * cos Shape.beta 
          Z = Shape.startPoint.Z + (Shape.leftPoint.Z - Shape.startPoint.Z) - cutterRadius * sin Shape.beta }

    // Gear end point
    let gearEndPoint = 
        { X = 
            if (Shape.gamma <= 0) then gearStartPoint.X + (Shape.rightPoint.X - Shape.leftPoint.X) + (cutterRadius - cutterRadius * cos Shape.beta)
            else                       gearStartPoint.X + (Shape.rightPoint.X - Shape.leftPoint.X) + ((cutterRadius - cutterRadius * cos Shape.beta) - (cutterRadius - cutterRadius * cos Shape.gamma))
          Z = 
            if (Shape.gamma <= 0) then 
                if (Shape.beta <> 0) then gearStartPoint.Z + (Shape.rightPoint.Z - Shape.leftPoint.Z) + (cutterRadius - cutterRadius * cos Shape.beta) / tan Shape.beta 
                else                     gearStartPoint.Z + (Shape.rightPoint.Z - Shape.leftPoint.Z)
            else                       
                if (Shape.beta <> 0) then gearStartPoint.Z + (Shape.rightPoint.Z - Shape.leftPoint.Z) + ((cutterRadius - cutterRadius * cos Shape.beta) - (cutterRadius - cutterRadius * cos Shape.gamma)) / tan Shape.beta 
                else                     gearStartPoint.Z + (Shape.rightPoint.Z - Shape.leftPoint.Z) }


module Draw = 

    open System
    open System.Drawing
    open System.Drawing.Drawing2D
    open System.Windows.Forms

    type Canvas() =
        inherit Control()
        override c.OnPaint(e:PaintEventArgs) =
            base.OnPaint(e)

            let g = e.Graphics

            let matrix = new Matrix()
            matrix.Rotate(-90f)
            matrix.Scale(4f, 4f, MatrixOrder.Append)
            matrix.Translate(200f, 400f, MatrixOrder.Append)

            // draw gear shape
            let path = new GraphicsPath()
            let shapePoints = 
                [|
                PointF(float32 Shape.startPoint.X, float32 Shape.startPoint.Z)
                PointF(float32 Shape.leftPoint.X, float32 Shape.leftPoint.Z)
                PointF(float32 Shape.rightPoint.X, float32 Shape.rightPoint.Z)
                PointF(float32 Shape.endPoint.X, float32 Shape.endPoint.Z)
                |]            
            path.AddLines(shapePoints)    
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Black, width = 1.0f), path)

            // draw cutter for normal start
            path.Reset()
            path.AddEllipse(float32 (Trajectory.normalPoint.X - Trajectory.cutterRadius), float32 (Trajectory.normalPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.normalPlungeInPoint.X - Trajectory.cutterRadius), float32 (Trajectory.normalPlungeInPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.normalPoint.X, float32 Trajectory.normalPoint.Z, float32 Trajectory.normalPlungeInPoint.X, float32 Trajectory.normalPlungeInPoint.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Red, width = 1.0f), path)

            // draw cutter for tangential start
            path.Reset()
            path.AddEllipse(float32 (Trajectory.tangentialPoint.X - Trajectory.cutterRadius), float32 (Trajectory.tangentialPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.tangentialInfeedPoint.X - Trajectory.cutterRadius), float32 (Trajectory.tangentialInfeedPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.tangentialPoint.X, float32 Trajectory.tangentialPoint.Z, float32 Trajectory.tangentialInfeedPoint.X, float32 Trajectory.tangentialInfeedPoint.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Blue, width = 1.0f), path)

            // draw cutter at entry end point
            path.Reset()
            path.AddEllipse(float32 (Trajectory.entryEndPoint.X - Trajectory.cutterRadius), float32 (Trajectory.entryEndPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.gearStartPoint.X - Trajectory.cutterRadius), float32 (Trajectory.gearStartPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.gearEndPoint.X - Trajectory.cutterRadius), float32 (Trajectory.gearEndPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.normalPoint.X, float32 Trajectory.normalPoint.Z, float32 Trajectory.entryEndPoint.X, float32 Trajectory.entryEndPoint.Z)
            path.AddLine(float32 Trajectory.entryEndPoint.X, float32 Trajectory.entryEndPoint.Z, float32 Trajectory.gearStartPoint.X, float32 Trajectory.gearStartPoint.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Green, width = 1.0f), path)

        override c.OnResize(e:EventArgs) =
            c.Refresh()
            
    let canvas = new Canvas(Dock = DockStyle.Fill)

    let form = new Form(Width = 400, Height = 400, Text = "NCGenerator - Draw")
    form.Controls.Add(canvas)

    form.Show()
    Application.Run(form)
