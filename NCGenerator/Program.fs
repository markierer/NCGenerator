module Shape = 

    // Start point (left)
    let startPointX = 30.0
    let startPointZ = 0.0

    // First point (left)
    let leftPointX = 50.0
    let leftPointZ = 10.0

    // Angle alpha
    let alpha = atan2 (leftPointX - startPointX) (leftPointZ - startPointZ)
    let alphaDegrees = atan2 (leftPointX - startPointX) (leftPointZ - startPointZ) * 180.0 / System.Math.PI

    // Second point (right)
    let rightPointX = 49.9
    let rightPointZ = 60.0

    let beta = atan2 (rightPointX - leftPointX) (rightPointZ - leftPointZ)
    let betaDegrees = atan2 (rightPointX - leftPointX) (rightPointZ - leftPointZ) * 180.0 / System.Math.PI

    // End point (right)
    let endPointX = 30.0
    let endPointZ = 70.0

    let gamma = atan2 (endPointX - rightPointX) (endPointZ - rightPointZ)
    let gammaDegrees = atan2 (endPointX - rightPointX) (endPointZ - rightPointZ) * 180.0 / System.Math.PI


module Trajectory = 
    
    // Cutter diameter
    let cutterDiameter = 40.0
    let cutterRadius = cutterDiameter / 2.0

    // Normal point
    let normalPointX = Shape.startPointX + cutterRadius * cos Shape.alpha
    let normalPointZ = Shape.startPointZ - cutterRadius * sin Shape.alpha

    // Normal plunge in point
    let plungeInX = 30.0
    let normalPlungeInPointX = normalPointX + plungeInX
    let normalPlungeInPointZ = normalPointZ - plungeInX * tan Shape.alpha

    // Tangential point
    let tangentialPointX = normalPointX - (cutterRadius - cutterRadius * sin Shape.alpha)
    let tangentialPointZ = normalPointZ - (cutterRadius - cutterRadius * sin Shape.alpha) * tan Shape.alpha

    // Tangential point with infeed Z
    let infeedZ = 20.0
    let tangentialInfeedPointX = tangentialPointX - infeedZ * tan Shape.alpha
    let tangentialInfeedPointZ = tangentialPointZ - infeedZ


module Draw = 

    open System.Drawing
    open System.Drawing.Drawing2D
    open System.Windows.Forms

    let ptsRook = 
        [|
        PointF(20.0f, 23.0f)
        PointF(20.0f, 50.0f)
        PointF(32.0f, 50.0f)

        PointF(32.0f, 85.0f)
        PointF(15.0f, 85.0f)
        PointF(15.0f, 95.0f)
        PointF(80.0f, 95.0f)
        PointF(80.0f, 85.0f)
        PointF(63.0f, 85.0f)

        PointF(63.0f, 50.0f)
        PointF(75.0f, 50.0f)
        PointF(75.0f, 23.0f)
        |]

    let form = new Form(Width = 400, Height = 400, Text = "NCGenerator - Draw")
    let g = form.GetGraphics
    let pen = new Pen(brush = Brushes.Black, width = 1.0f)
    let path = new GraphicsPath()
    path.StartFigure()
    path.AddLines(ptsRook)
    path.CloseFigure()

    g.FillPath(Brushes.LightGray, path)
    g.DrawPath(Pens.Brown, path)

    form.Show()
    Application.Run(form)
