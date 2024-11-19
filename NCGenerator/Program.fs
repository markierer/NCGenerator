type Point = { X: float; Z: float }
let degrees radiant = radiant * 180.0 / System.Math.PI

module Shape = 

    let point1 = 
        { X = 30.0 
          Z = 0.0 }

    let point2 = 
        { X = 40.0 
          Z = 10.0 }

    let alpha = atan2 (point2.X - point1.X) (point2.Z - point1.Z)
    let alphaDegrees = degrees alpha

    let point3 = 
        { X = 50.0 
          Z = 60.0 }

    let beta = atan2 (point3.X - point2.X) (point3.Z - point2.Z)
    let betaDegrees = degrees beta

    let point4 = 
        { X = 20.0 
          Z = 70.0 }

    let gamma = atan2 (point4.X - point3.X) (point4.Z - point3.Z)
    let gammaDegrees = degrees gamma


module Trajectory = 

    // Cutter diameter
    let cutterDiameter = 20.0
    let cutterRadius = cutterDiameter / 2.0

    let intersection (point1: Point) (point2: Point) (point3: Point) (point4: Point) : Point =
        let div1 = point2.Z - point1.Z
        let m1 = (point2.X - point1.X) / div1
        let b1 = point1.X - m1 * point1.Z
        let div2 = point4.Z - point3.Z
        let m2 = (point4.X - point3.X) / div2
        let b2 = point3.X - m2 * point3.Z
        let div = m1 - m2
        let z = (b2 - b1) / div
        let x = m1 * z + b1
        if System.Double.IsNaN(x) then point2
        elif System.Double.IsNaN(z) then point2
        else
            let retVal = 
                { X = x
                  Z = z }
            retVal

    // Start point
    let startPoint = 
        { X = Shape.point1.X + cutterRadius * cos Shape.alpha 
          Z = Shape.point1.Z - cutterRadius * sin Shape.alpha }

    // Example plunge in point for normal start
    let plungeInX = 8.94
    let plungeInPoint = 
        { X = startPoint.X + plungeInX * cos Shape.alpha
          Z = startPoint.Z - plungeInX * sin Shape.alpha }

    // Tangential start point
    // Where the cutter's 3 o'clock would touch the part
    let tangentialStartPoint = 
        { X = startPoint.X - (cutterRadius - cutterRadius * sin Shape.alpha) * tan Shape.alpha
          Z = startPoint.Z - (cutterRadius - cutterRadius * sin Shape.alpha) }

    // Example infeed point for tangential start
    let infeedZ = 8.94
    let infeedPoint = 
        { X = tangentialStartPoint.X - infeedZ * sin Shape.alpha
          Z = tangentialStartPoint.Z - infeedZ * cos Shape.alpha }

    // End point after first segment
    let segmentEndPoint1 = 
        { X = Shape.point2.X + cutterRadius * cos Shape.alpha 
          Z = Shape.point2.Z - cutterRadius * sin Shape.alpha }

    // Gear start point
    let startPointGear = 
        { X = Shape.point2.X + cutterRadius * cos Shape.beta 
          Z = Shape.point2.Z - cutterRadius * sin Shape.beta }

    // Gear end point
    let endPointGear = 
        { X = Shape.point3.X + cutterRadius * cos Shape.beta 
          Z = Shape.point3.Z - cutterRadius * sin Shape.beta }

    // Start point before third segment
    let segmentStartPoint3 = 
        { X = Shape.point3.X + cutterRadius * cos Shape.gamma 
          Z = Shape.point3.Z - cutterRadius * sin Shape.gamma }

    // End point after gear segment
    let endPoint = 
        { X = Shape.point4.X + cutterRadius * cos Shape.gamma 
          Z = Shape.point4.Z - cutterRadius * sin Shape.gamma }

    // Example plunge in point for normal start
    let plungeOutX = 8.94
    let plungeOutPoint = 
        { X = endPoint.X + plungeOutX * cos Shape.gamma
          Z = endPoint.Z - plungeOutX * sin Shape.gamma }

    // Transition point between first segment and gear segment
    let transitionPoint1 = intersection startPoint segmentEndPoint1 startPointGear endPointGear

    // Transition point between gear segment and third segment
    let transitionPoint2 = intersection startPointGear endPointGear segmentStartPoint3 endPoint


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
                PointF(float32 Shape.point1.X, float32 Shape.point1.Z)
                PointF(float32 Shape.point2.X, float32 Shape.point2.Z)
                PointF(float32 Shape.point3.X, float32 Shape.point3.Z)
                PointF(float32 Shape.point4.X, float32 Shape.point4.Z)
                |]            
            path.AddLines(shapePoints)    
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Black, width = 1.0f), path)

            // draw cutter at normal start
            path.Reset()
            path.AddEllipse(float32 (Trajectory.startPoint.X - Trajectory.cutterRadius), float32 (Trajectory.startPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.plungeInPoint.X - Trajectory.cutterRadius), float32 (Trajectory.plungeInPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.startPoint.X, float32 Trajectory.startPoint.Z, float32 Trajectory.plungeInPoint.X, float32 Trajectory.plungeInPoint.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Red, width = 1.0f), path)

            // draw cutter at tangential start
            path.Reset()
            path.AddEllipse(float32 (Trajectory.tangentialStartPoint.X - Trajectory.cutterRadius), float32 (Trajectory.tangentialStartPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.infeedPoint.X - Trajectory.cutterRadius), float32 (Trajectory.infeedPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.tangentialStartPoint.X, float32 Trajectory.tangentialStartPoint.Z, float32 Trajectory.infeedPoint.X, float32 Trajectory.infeedPoint.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Blue, width = 1.0f), path)

            // draw cutter at gear start
            path.Reset()
            path.AddEllipse(float32 (Trajectory.segmentEndPoint1.X - Trajectory.cutterRadius), float32 (Trajectory.segmentEndPoint1.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.transitionPoint1.X - Trajectory.cutterRadius), float32 (Trajectory.transitionPoint1.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.startPointGear.X - Trajectory.cutterRadius), float32 (Trajectory.startPointGear.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.startPoint.X, float32 Trajectory.startPoint.Z, float32 Trajectory.transitionPoint1.X, float32 Trajectory.transitionPoint1.Z)
            path.AddLine(float32 Trajectory.transitionPoint1.X, float32 Trajectory.transitionPoint1.Z, float32 Trajectory.startPointGear.X, float32 Trajectory.startPointGear.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Green, width = 1.0f), path)

            // draw cutter at gear end
            path.Reset()
            path.AddEllipse(float32 (Trajectory.endPointGear.X - Trajectory.cutterRadius), float32 (Trajectory.endPointGear.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.transitionPoint2.X - Trajectory.cutterRadius), float32 (Trajectory.transitionPoint2.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.segmentStartPoint3.X - Trajectory.cutterRadius), float32 (Trajectory.segmentStartPoint3.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.startPointGear.X, float32 Trajectory.startPointGear.Z, float32 Trajectory.endPointGear.X, float32 Trajectory.endPointGear.Z)
            path.AddLine(float32 Trajectory.transitionPoint2.X, float32 Trajectory.transitionPoint2.Z, float32 Trajectory.segmentStartPoint3.X, float32 Trajectory.segmentStartPoint3.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Violet, width = 1.0f), path)

            // draw cutter at end
            path.Reset()
            path.AddEllipse(float32 (Trajectory.endPoint.X - Trajectory.cutterRadius), float32 (Trajectory.endPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddEllipse(float32 (Trajectory.plungeOutPoint.X - Trajectory.cutterRadius), float32 (Trajectory.plungeOutPoint.Z - Trajectory.cutterRadius), float32 Trajectory.cutterDiameter, float32 Trajectory.cutterDiameter)
            path.AddLine(float32 Trajectory.segmentStartPoint3.X, float32 Trajectory.segmentStartPoint3.Z, float32 Trajectory.endPoint.X, float32 Trajectory.endPoint.Z)
            path.AddLine(float32 Trajectory.endPoint.X, float32 Trajectory.endPoint.Z, float32 Trajectory.plungeOutPoint.X, float32 Trajectory.plungeOutPoint.Z)
            path.Transform(matrix)
            g.DrawPath(new Pen(brush = Brushes.Orange, width = 1.0f), path)

        override c.OnResize(e:EventArgs) =
            c.Refresh()
            
    let canvas = new Canvas(Dock = DockStyle.Fill)

    let form = new Form(Width = 400, Height = 400, Text = "NCGenerator - Draw")
    form.Controls.Add(canvas)

    form.Show()
    Application.Run(form)
