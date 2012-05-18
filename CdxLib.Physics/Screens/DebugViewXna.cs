using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CdxLib.Core.Graphics;
using CdxLib.Core.ScreenManager;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Physics.Screens
{
    /// <summary>
    ///   A debug view that works in XNA.
    ///   A debug view shows you what happens inside the physics engine. You can view
    ///   bodies, joints, fixtures and more.
    /// </summary>
    public class DebugViewXna : DebugView, IDisposable
    {
        private const int MaxContactPoints = 2048;
        public const int CircleSegments = 16;
        private readonly Vector2[] _background = new Vector2[4];
        private readonly List<float> _graphValues = new List<float>();
        private readonly ContactPoint[] _points = new ContactPoint[MaxContactPoints];
        private readonly Vector2[] _tempVertices = new Vector2[Settings.MaxPolygonVertices];
        public bool AdaptiveLimits = true;
        public Vector2 DebugPanelPosition = new Vector2(10, 10);
        public Color DefaultShapeColor = new Color(0.9f, 0.7f, 0.7f);
        public bool Enabled;
        public Color InactiveShapeColor = new Color(0.5f, 0.5f, 0.3f);
        public Color KinematicShapeColor = new Color(0.5f, 0.5f, 0.9f);
        public int MaximumValue = 1000;
        public int MinimumValue;
        public Rectangle PerformancePanelBounds = new Rectangle(250, 100, 200, 100);
        public Color SleepingShapeColor = new Color(0.6f, 0.6f, 0.6f);
        public Color StaticShapeColor = new Color(0.5f, 0.9f, 0.5f);
        public Color TextColor = Color.White;
        public int ValuesToGraph = 500;
        private int _avg;
        private SpriteBatch _batch;
        private Matrix _localProjection;
        private Matrix _localView;
        private int _max;
        private int _min;
        private int _pointCount;
        private PrimitiveBatch _primitiveBatch;
        private ScreenManager _screenManager;
        private List<StringData> _stringData;

        /// <summary>
        /// </summary>
        /// <param name = "world"></param>
        public DebugViewXna(World world)
            : base(world)
        {
            world.ContactManager.PreSolve += PreSolve;
            AppendFlags(DebugViewFlags.Shape);
            AppendFlags(DebugViewFlags.Controllers);
            AppendFlags(DebugViewFlags.Joint);
        }

        #region IDisposable Members

        public void Dispose()
        {
            World.ContactManager.PreSolve -= PreSolve;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name = "projection"></param>
        /// <param name = "view"></param>
        public void BeginCustomDraw(ref Matrix projection, ref Matrix view)
        {
            _primitiveBatch.Begin(ref projection, ref view);
        }

        /// <summary>
        /// </summary>
        public void EndCustomDraw()
        {
            _primitiveBatch.End();
        }

        /// <summary>
        /// </summary>
        /// <param name = "contact"></param>
        /// <param name = "oldManifold"></param>
        private void PreSolve(Contact contact, ref Manifold oldManifold)
        {
            if ((Flags & DebugViewFlags.ContactPoints) != DebugViewFlags.ContactPoints) return;
            Manifold manifold = contact.Manifold;

            if (manifold.PointCount == 0)
            {
                return;
            }

            Fixture fixtureA = contact.FixtureA;

            FixedArray2<PointState> state1, state2;
            Collision.GetPointStates(out state1, out state2, ref oldManifold, ref manifold);

            FixedArray2<Vector2> points;
            Vector2 normal;
            contact.GetWorldManifold(out normal, out points);

            for (int i = 0; i < manifold.PointCount && _pointCount < MaxContactPoints; ++i)
            {
                if (fixtureA == null)
                {
                    _points[i] = new ContactPoint();
                }
                ContactPoint cp = _points[_pointCount];
                cp.Position = points[i];
                cp.Normal = normal;
                cp.State = state2[i];
                _points[_pointCount] = cp;
                ++_pointCount;
            }
        }

        /// <summary>
        ///   Call this to draw shapes and other debug draw data.
        /// </summary>
        private void DrawDebugData()
        {
            if ((Flags & DebugViewFlags.Shape) == DebugViewFlags.Shape)
            {
                foreach (Body b in World.BodyList)
                {
                    Transform xf;
                    b.GetTransform(out xf);
                    foreach (Fixture f in b.FixtureList)
                    {
                        if (b.Enabled == false)
                        {
                            DrawShape(f, xf, InactiveShapeColor);
                        }
                        else if (b.BodyType == BodyType.Static)
                        {
                            DrawShape(f, xf, StaticShapeColor);
                        }
                        else if (b.BodyType == BodyType.Kinematic)
                        {
                            DrawShape(f, xf, KinematicShapeColor);
                        }
                        else if (b.Awake == false)
                        {
                            DrawShape(f, xf, SleepingShapeColor);
                        }
                        else
                        {
                            DrawShape(f, xf, DefaultShapeColor);
                        }
                    }
                }
            }
            if ((Flags & DebugViewFlags.ContactPoints) == DebugViewFlags.ContactPoints)
            {
                const float axisScale = 0.3f;

                for (int i = 0; i < _pointCount; ++i)
                {
                    ContactPoint point = _points[i];

                    switch (point.State)
                    {
                        case PointState.Add:
                            DrawPoint(point.Position, 0.1f, new Color(0.3f, 0.95f, 0.3f));
                            break;
                        case PointState.Persist:
                            DrawPoint(point.Position, 0.1f, new Color(0.3f, 0.3f, 0.95f));
                            break;
                    }

                    if ((Flags & DebugViewFlags.ContactNormals) != DebugViewFlags.ContactNormals) continue;
                    Vector2 p1 = point.Position;
                    Vector2 p2 = p1 + axisScale*point.Normal;
                    DrawSegment(p1, p2, new Color(0.4f, 0.9f, 0.4f));
                }
                _pointCount = 0;
            }
            if ((Flags & DebugViewFlags.PolygonPoints) == DebugViewFlags.PolygonPoints)
            {
                foreach (Body body in World.BodyList)
                {
                    foreach (PolygonShape polygon in body.FixtureList.Select(f => f.Shape).OfType<PolygonShape>())
                    {
                        Transform xf;
                        body.GetTransform(out xf);

                        foreach (Vector2 tmp in polygon.Vertices.Select(t => MathUtils.Multiply(ref xf, t)))
                        {
                            DrawPoint(tmp, 0.1f, Color.Red);
                        }
                    }
                }
            }
            if ((Flags & DebugViewFlags.Joint) == DebugViewFlags.Joint)
            {
                foreach (Joint j in World.JointList)
                {
                    DrawJoint(j);
                }
            }
            if ((Flags & DebugViewFlags.Pair) == DebugViewFlags.Pair)
            {
                var color = new Color(0.3f, 0.9f, 0.9f);
                foreach (Contact c in World.ContactManager.ContactList)
                {
                    Fixture fixtureA = c.FixtureA;
                    Fixture fixtureB = c.FixtureB;

                    AABB aabbA;
                    fixtureA.GetAABB(out aabbA, 0);
                    AABB aabbB;
                    fixtureB.GetAABB(out aabbB, 0);

                    Vector2 cA = aabbA.Center;
                    Vector2 cB = aabbB.Center;

                    DrawSegment(cA, cB, color);
                }
            }
            if ((Flags & DebugViewFlags.AABB) == DebugViewFlags.AABB)
            {
                var color = new Color(0.9f, 0.3f, 0.9f);
                IBroadPhase bp = World.ContactManager.BroadPhase;

                foreach (Fixture f in from b in World.BodyList where b.Enabled from f in b.FixtureList select f)
                {
                    for (int t = 0; t < f.ProxyCount; ++t)
                    {
                        FixtureProxy proxy = f.Proxies[t];
                        AABB aabb;
                        bp.GetFatAABB(proxy.ProxyId, out aabb);

                        DrawAABB(ref aabb, color);
                    }
                }
            }
            if ((Flags & DebugViewFlags.CenterOfMass) == DebugViewFlags.CenterOfMass)
            {
                foreach (Body b in World.BodyList)
                {
                    Transform xf;
                    b.GetTransform(out xf);
                    xf.Position = b.WorldCenter;
                    DrawTransform(ref xf);
                }
            }
            if ((Flags & DebugViewFlags.Controllers) == DebugViewFlags.Controllers)
            {
                foreach (Controller controller in World.ControllerList)
                {
                    var buoyancy = controller as BuoyancyController;
                    if (buoyancy == null) continue;
                    AABB container = buoyancy.Container;
                    DrawAABB(ref container, Color.LightBlue);
                }
            }
            if ((Flags & DebugViewFlags.DebugPanel) == DebugViewFlags.DebugPanel)
            {
                DrawDebugPanel();
            }
        }

        /// <summary>
        /// </summary>
        private void DrawPerformanceGraph()
        {
            _graphValues.Add(World.UpdateTime);

            if (_graphValues.Count > ValuesToGraph + 1)
                _graphValues.RemoveAt(0);

            float x = PerformancePanelBounds.X;
            float deltaX = PerformancePanelBounds.Width/(float) ValuesToGraph;
            float yScale = PerformancePanelBounds.Bottom - (float) PerformancePanelBounds.Top;

            // we must have at least 2 values to start rendering
            if (_graphValues.Count > 2)
            {
                _max = (int) _graphValues.Max();
                _avg = (int) _graphValues.Average();
                _min = (int) _graphValues.Min();

                if (AdaptiveLimits)
                {
                    MaximumValue = _max;
                    MinimumValue = 0;
                }

                // start at last value (newest value added)
                // continue until no values are left
                for (int i = _graphValues.Count - 1; i > 0; i--)
                {
                    float y1 = PerformancePanelBounds.Bottom -
                               ((_graphValues[i]/(MaximumValue - MinimumValue))*yScale);
                    float y2 = PerformancePanelBounds.Bottom -
                               ((_graphValues[i - 1]/(MaximumValue - MinimumValue))*yScale);

                    var x1 =
                        new Vector2(MathHelper.Clamp(x, PerformancePanelBounds.Left, PerformancePanelBounds.Right),
                                    MathHelper.Clamp(y1, PerformancePanelBounds.Top, PerformancePanelBounds.Bottom));

                    var x2 =
                        new Vector2(
                            MathHelper.Clamp(x + deltaX, PerformancePanelBounds.Left, PerformancePanelBounds.Right),
                            MathHelper.Clamp(y2, PerformancePanelBounds.Top, PerformancePanelBounds.Bottom));

                    DrawSegment(x1, x2, Color.LightGreen);

                    x += deltaX;
                }
            }

            DrawString(PerformancePanelBounds.Right + 10, PerformancePanelBounds.Top, "Max: " + _max);
            DrawString(PerformancePanelBounds.Right + 10, PerformancePanelBounds.Center.Y - 7, "Avg: " + _avg);
            DrawString(PerformancePanelBounds.Right + 10, PerformancePanelBounds.Bottom - 15, "Min: " + _min);

            //Draw background.
            _background[0] = new Vector2(PerformancePanelBounds.X, PerformancePanelBounds.Y);
            _background[1] = new Vector2(PerformancePanelBounds.X,
                                         PerformancePanelBounds.Y + PerformancePanelBounds.Height);
            _background[2] = new Vector2(PerformancePanelBounds.X + PerformancePanelBounds.Width,
                                         PerformancePanelBounds.Y + PerformancePanelBounds.Height);
            _background[3] = new Vector2(PerformancePanelBounds.X + PerformancePanelBounds.Width,
                                         PerformancePanelBounds.Y);

            DrawSolidPolygon(_background, 4, Color.DarkGray, true);
        }

        /// <summary>
        /// </summary>
        private void DrawDebugPanel()
        {
            int fixtures = World.BodyList.Sum(t => t.FixtureList.Count);
            var x = (int) DebugPanelPosition.X;
            var y = (int) DebugPanelPosition.Y;

            DrawString(x, y, "Objects:" +
                             "\n- Bodies: " + World.BodyList.Count +
                             "\n- Fixtures: " + fixtures +
                             "\n- Contacts: " + World.ContactList.Count +
                             "\n- Joints: " + World.JointList.Count +
                             "\n- Controllers: " + World.ControllerList.Count +
                             "\n- Proxies: " + World.ProxyCount);

            DrawString(x + 110, y, "Update time:" +
                                   "\n- Body: " + World.SolveUpdateTime +
                                   "\n- Contact: " + World.ContactsUpdateTime +
                                   "\n- CCD: " + World.ContinuousPhysicsTime +
                                   "\n- Joint: " + World.Island.JointUpdateTime +
                                   "\n- Controller: " + World.ControllersUpdateTime +
                                   "\n- Total: " + World.UpdateTime);
        }

        /// <summary>
        /// </summary>
        /// <param name = "aabb"></param>
        /// <param name = "color"></param>
        public void DrawAABB(ref AABB aabb, Color color)
        {
            var verts = new Vector2[4];
            verts[0] = new Vector2(aabb.LowerBound.X, aabb.LowerBound.Y);
            verts[1] = new Vector2(aabb.UpperBound.X, aabb.LowerBound.Y);
            verts[2] = new Vector2(aabb.UpperBound.X, aabb.UpperBound.Y);
            verts[3] = new Vector2(aabb.LowerBound.X, aabb.UpperBound.Y);

            DrawPolygon(verts, 4, color);
        }

        private void DrawJoint(Joint joint)
        {
            if (!joint.Enabled)
                return;

            Body b1 = joint.BodyA;
            Body b2 = joint.BodyB;
            Transform xf1;
            b1.GetTransform(out xf1);

            Vector2 x2 = Vector2.Zero;

            // WIP David
            if (!joint.IsFixedType())
            {
                Transform xf2;
                b2.GetTransform(out xf2);
                x2 = xf2.Position;
            }

            Vector2 p1 = joint.WorldAnchorA;
            Vector2 p2 = joint.WorldAnchorB;
            Vector2 x1 = xf1.Position;

            var color = new Color(0.5f, 0.8f, 0.8f);

            switch (joint.JointType)
            {
                case JointType.Distance:
                    DrawSegment(p1, p2, color);
                    break;
                case JointType.Pulley:
                    var pulley = (PulleyJoint) joint;
                    Vector2 s1 = pulley.GroundAnchorA;
                    Vector2 s2 = pulley.GroundAnchorB;
                    DrawSegment(s1, p1, color);
                    DrawSegment(s2, p2, color);
                    DrawSegment(s1, s2, color);
                    break;
                case JointType.FixedMouse:
                    DrawPoint(p1, 0.5f, new Color(0.0f, 1.0f, 0.0f));
                    DrawSegment(p1, p2, new Color(0.8f, 0.8f, 0.8f));
                    break;
                case JointType.Revolute:
                    //DrawSegment(x2, p1, color);
                    DrawSegment(p2, p1, color);
                    DrawSolidCircle(p2, 0.1f, Vector2.Zero, Color.Red);
                    DrawSolidCircle(p1, 0.1f, Vector2.Zero, Color.Blue);
                    break;
                case JointType.FixedAngle:
                    //Should not draw anything.
                    break;
                case JointType.FixedRevolute:
                    DrawSegment(x1, p1, color);
                    DrawSolidCircle(p1, 0.1f, Vector2.Zero, Color.Pink);
                    break;
                case JointType.FixedLine:
                    DrawSegment(x1, p1, color);
                    DrawSegment(p1, p2, color);
                    break;
                case JointType.FixedDistance:
                    DrawSegment(x1, p1, color);
                    DrawSegment(p1, p2, color);
                    break;
                case JointType.FixedPrismatic:
                    DrawSegment(x1, p1, color);
                    DrawSegment(p1, p2, color);
                    break;
                case JointType.Gear:
                    DrawSegment(x1, x2, color);
                    break;
                    //case JointType.Weld:
                    //    break;
                default:
                    DrawSegment(x1, p1, color);
                    DrawSegment(p1, p2, color);
                    DrawSegment(x2, p2, color);
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "fixture"></param>
        /// <param name = "xf"></param>
        /// <param name = "color"></param>
        public void DrawShape(Fixture fixture, Transform xf, Color color)
        {
            switch (fixture.ShapeType)
            {
                case ShapeType.Circle:
                    {
                        var circle = (CircleShape) fixture.Shape;

                        Vector2 center = MathUtils.Multiply(ref xf, circle.Position);
                        float radius = circle.Radius;
                        Vector2 axis = xf.R.Col1;

                        DrawSolidCircle(center, radius, axis, color);
                    }
                    break;

                case ShapeType.Polygon:
                    {
                        var poly = (PolygonShape) fixture.Shape;
                        int vertexCount = poly.Vertices.Count;
                        Debug.Assert(vertexCount <= Settings.MaxPolygonVertices);

                        for (int i = 0; i < vertexCount; ++i)
                        {
                            _tempVertices[i] = MathUtils.Multiply(ref xf, poly.Vertices[i]);
                        }

                        DrawSolidPolygon(_tempVertices, vertexCount, color);
                    }
                    break;


                case ShapeType.Edge:
                    {
                        var edge = (EdgeShape) fixture.Shape;
                        Vector2 v1 = MathUtils.Multiply(ref xf, edge.Vertex1);
                        Vector2 v2 = MathUtils.Multiply(ref xf, edge.Vertex2);
                        DrawSegment(v1, v2, color);
                    }
                    break;

                case ShapeType.Loop:
                    {
                        var loop = (LoopShape) fixture.Shape;
                        int count = loop.Vertices.Count;

                        Vector2 v1 = MathUtils.Multiply(ref xf, loop.Vertices[count - 1]);
                        DrawCircle(v1, 0.05f, color);
                        for (int i = 0; i < count; ++i)
                        {
                            Vector2 v2 = MathUtils.Multiply(ref xf, loop.Vertices[i]);
                            DrawSegment(v1, v2, color);
                            v1 = v2;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "vertices"></param>
        /// <param name = "count"></param>
        /// <param name = "red"></param>
        /// <param name = "green"></param>
        /// <param name = "blue"></param>
        public override void DrawPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawPolygon(vertices, count, new Color(red, green, blue));
        }

        /// <summary>
        /// </summary>
        /// <param name = "vertices"></param>
        /// <param name = "count"></param>
        /// <param name = "color"></param>
        public void DrawPolygon(Vector2[] vertices, int count, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            for (int i = 0; i < count - 1; i++)
            {
                _primitiveBatch.AddVertex(vertices[i], color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(vertices[i + 1], color, PrimitiveType.LineList);
            }

            _primitiveBatch.AddVertex(vertices[count - 1], color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(vertices[0], color, PrimitiveType.LineList);
        }

        /// <summary>
        /// </summary>
        /// <param name = "vertices"></param>
        /// <param name = "count"></param>
        /// <param name = "red"></param>
        /// <param name = "green"></param>
        /// <param name = "blue"></param>
        public override void DrawSolidPolygon(Vector2[] vertices, int count, float red, float green, float blue)
        {
            DrawSolidPolygon(vertices, count, new Color(red, green, blue), true);
        }

        /// <summary>
        /// </summary>
        /// <param name = "vertices"></param>
        /// <param name = "count"></param>
        /// <param name = "color"></param>
        public void DrawSolidPolygon(Vector2[] vertices, int count, Color color)
        {
            DrawSolidPolygon(vertices, count, color, true);
        }

        /// <summary>
        /// </summary>
        /// <param name = "vertices"></param>
        /// <param name = "count"></param>
        /// <param name = "color"></param>
        /// <param name = "outline"></param>
        public void DrawSolidPolygon(Vector2[] vertices, int count, Color color, bool outline)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            if (count == 2)
            {
                DrawPolygon(vertices, count, color);
                return;
            }

            Color colorFill = color*(outline ? 0.5f : 1.0f);

            for (int i = 1; i < count - 1; i++)
            {
                _primitiveBatch.AddVertex(vertices[0], colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(vertices[i], colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(vertices[i + 1], colorFill, PrimitiveType.TriangleList);
            }

            if (outline)
            {
                DrawPolygon(vertices, count, color);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "center"></param>
        /// <param name = "radius"></param>
        /// <param name = "red"></param>
        /// <param name = "green"></param>
        /// <param name = "blue"></param>
        public override void DrawCircle(Vector2 center, float radius, float red, float green, float blue)
        {
            DrawCircle(center, radius, new Color(red, green, blue));
        }

        /// <summary>
        /// </summary>
        /// <param name = "center"></param>
        /// <param name = "radius"></param>
        /// <param name = "color"></param>
        public void DrawCircle(Vector2 center, float radius, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            const double increment = Math.PI*2.0/CircleSegments;
            double theta = 0.0;

            for (int i = 0; i < CircleSegments; i++)
            {
                Vector2 v1 = center + radius*new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                Vector2 v2 = center +
                             radius*
                             new Vector2((float) Math.Cos(theta + increment), (float) Math.Sin(theta + increment));

                _primitiveBatch.AddVertex(v1, color, PrimitiveType.LineList);
                _primitiveBatch.AddVertex(v2, color, PrimitiveType.LineList);

                theta += increment;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "center"></param>
        /// <param name = "radius"></param>
        /// <param name = "axis"></param>
        /// <param name = "red"></param>
        /// <param name = "green"></param>
        /// <param name = "blue"></param>
        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float green,
                                             float blue)
        {
            DrawSolidCircle(center, radius, axis, new Color(red, green, blue));
        }

        /// <summary>
        /// </summary>
        /// <param name = "center"></param>
        /// <param name = "radius"></param>
        /// <param name = "axis"></param>
        /// <param name = "color"></param>
        public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            const double increment = Math.PI*2.0/CircleSegments;
            double theta = 0.0;

            Color colorFill = color*0.5f;

            Vector2 v0 = center + radius*new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < CircleSegments - 1; i++)
            {
                Vector2 v1 = center + radius*new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                Vector2 v2 = center +
                             radius*
                             new Vector2((float) Math.Cos(theta + increment), (float) Math.Sin(theta + increment));

                _primitiveBatch.AddVertex(v0, colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v1, colorFill, PrimitiveType.TriangleList);
                _primitiveBatch.AddVertex(v2, colorFill, PrimitiveType.TriangleList);

                theta += increment;
            }
            DrawCircle(center, radius, color);

            DrawSegment(center, center + axis*radius, color);
        }

        /// <summary>
        /// </summary>
        /// <param name = "start"></param>
        /// <param name = "end"></param>
        /// <param name = "red"></param>
        /// <param name = "green"></param>
        /// <param name = "blue"></param>
        public override void DrawSegment(Vector2 start, Vector2 end, float red, float green, float blue)
        {
            DrawSegment(start, end, new Color(red, green, blue));
        }

        /// <summary>
        /// </summary>
        /// <param name = "start"></param>
        /// <param name = "end"></param>
        /// <param name = "color"></param>
        public void DrawSegment(Vector2 start, Vector2 end, Color color)
        {
            if (!_primitiveBatch.IsReady())
            {
                throw new InvalidOperationException("BeginCustomDraw must be called before drawing anything.");
            }
            _primitiveBatch.AddVertex(start, color, PrimitiveType.LineList);
            _primitiveBatch.AddVertex(end, color, PrimitiveType.LineList);
        }

        /// <summary>
        /// </summary>
        /// <param name = "transform"></param>
        public override void DrawTransform(ref Transform transform)
        {
            const float axisScale = 0.4f;
            Vector2 p1 = transform.Position;

            Vector2 p2 = p1 + axisScale*transform.R.Col1;
            DrawSegment(p1, p2, Color.Red);

            p2 = p1 + axisScale*transform.R.Col2;
            DrawSegment(p1, p2, Color.Green);
        }

        /// <summary>
        /// </summary>
        /// <param name = "p"></param>
        /// <param name = "size"></param>
        /// <param name = "color"></param>
        public void DrawPoint(Vector2 p, float size, Color color)
        {
            var verts = new Vector2[4];
            float hs = size/2.0f;
            verts[0] = p + new Vector2(-hs, -hs);
            verts[1] = p + new Vector2(hs, -hs);
            verts[2] = p + new Vector2(hs, hs);
            verts[3] = p + new Vector2(-hs, hs);

            DrawSolidPolygon(verts, 4, color, true);
        }

        /// <summary>
        /// </summary>
        /// <param name = "x"></param>
        /// <param name = "y"></param>
        /// <param name = "s"></param>
        /// <param name = "args"></param>
        public void DrawString(int x, int y, string s, params object[] args)
        {
            _stringData.Add(new StringData(x, y, s, args, TextColor));
        }

        /// <summary>
        /// </summary>
        /// <param name = "start"></param>
        /// <param name = "end"></param>
        /// <param name = "length"></param>
        /// <param name = "width"></param>
        /// <param name = "drawStartIndicator"></param>
        /// <param name = "color"></param>
        public void DrawArrow(Vector2 start, Vector2 end, float length, float width, bool drawStartIndicator,
                              Color color)
        {
            // Draw connection segment between start- and end-point
            DrawSegment(start, end, color);

            // Precalculate halfwidth
            float halfWidth = width/2;

            // Create directional reference
            Vector2 rotation = (start - end);
            rotation.Normalize();

            // Calculate angle of directional vector
            var angle = (float) Math.Atan2(rotation.X, -rotation.Y);
            // Create matrix for rotation
            Matrix rotMatrix = Matrix.CreateRotationZ(angle);
            // Create translation matrix for end-point
            Matrix endMatrix = Matrix.CreateTranslation(end.X, end.Y, 0);

            // Setup arrow end shape
            var verts = new Vector2[3];
            verts[0] = new Vector2(0, 0);
            verts[1] = new Vector2(-halfWidth, -length);
            verts[2] = new Vector2(halfWidth, -length);

            // Rotate end shape
            Vector2.Transform(verts, ref rotMatrix, verts);
            // Translate end shape
            Vector2.Transform(verts, ref endMatrix, verts);

            // Draw arrow end shape
            DrawSolidPolygon(verts, 3, color, false);

            if (drawStartIndicator)
            {
                // Create translation matrix for start
                Matrix startMatrix = Matrix.CreateTranslation(start.X, start.Y, 0);
                // Setup arrow start shape
                var baseVerts = new Vector2[4];
                baseVerts[0] = new Vector2(-halfWidth, length/4);
                baseVerts[1] = new Vector2(halfWidth, length/4);
                baseVerts[2] = new Vector2(halfWidth, 0);
                baseVerts[3] = new Vector2(-halfWidth, 0);

                // Rotate start shape
                Vector2.Transform(baseVerts, ref rotMatrix, baseVerts);
                // Translate start shape
                Vector2.Transform(baseVerts, ref startMatrix, baseVerts);
                // Draw start shape
                DrawSolidPolygon(baseVerts, 4, color, false);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "projection"></param>
        /// <param name = "view"></param>
        public void RenderDebugData(ref Matrix projection, ref Matrix view)
        {
            if (!Enabled)
            {
                return;
            }

            //Nothing is enabled - don't draw the debug view.
            if (Flags == 0)
                return;

            _screenManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            _screenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _primitiveBatch.Begin(ref projection, ref view);
            DrawDebugData();
            _primitiveBatch.End();

            if ((Flags & DebugViewFlags.PerformanceGraph) == DebugViewFlags.PerformanceGraph)
            {
                _primitiveBatch.Begin(ref _localProjection, ref _localView);
                DrawPerformanceGraph();
                _primitiveBatch.End();
            }

            // begin the sprite batch effect
            _batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // draw any strings we have
            for (int i = 0; i < _stringData.Count; i++)
            {
                _batch.DrawString(_screenManager.SpriteFonts.DebugFont,
                                  string.Format(_stringData[i].S, _stringData[i].Args),
                                  new Vector2(_stringData[i].X + 1f, _stringData[i].Y + 1f), Color.Black);
                _batch.DrawString(_screenManager.SpriteFonts.DebugFont,
                                  string.Format(_stringData[i].S, _stringData[i].Args),
                                  new Vector2(_stringData[i].X, _stringData[i].Y), _stringData[i].Color);
            }
            // end the sprite batch effect
            _batch.End();

            _stringData.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name = "projection"></param>
        public void RenderDebugData(ref Matrix projection)
        {
            if (!Enabled)
            {
                return;
            }
            Matrix view = Matrix.Identity;
            RenderDebugData(ref projection, ref view);
        }

        /// <summary>
        /// </summary>
        /// <param name="screenManager"></param>
        public void LoadContent(ScreenManager screenManager)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _screenManager = screenManager;
            _batch = new SpriteBatch(_screenManager.GraphicsDevice);
            _primitiveBatch = new PrimitiveBatch(_screenManager.GraphicsDevice, 1000);
            _stringData = new List<StringData>();

            _localProjection = Matrix.CreateOrthographicOffCenter(0f, _screenManager.GraphicsDevice.Viewport.Width,
                                                                  _screenManager.GraphicsDevice.Viewport.Height,
                                                                  0f, 0f, 1f);
            _localView = Matrix.Identity;
        }

        #region Nested type: ContactPoint

        private struct ContactPoint
        {
            public Vector2 Normal;
            public Vector2 Position;
            public PointState State;
        }

        #endregion

        #region Nested type: StringData

        private struct StringData
        {
            public readonly object[] Args;
            public readonly Color Color;
            public readonly string S;
            public readonly int X;
            public readonly int Y;

            public StringData(int x, int y, string s, object[] args, Color color)
            {
                X = x;
                Y = y;
                S = s;
                Args = args;
                Color = color;
            }
        }

        #endregion
    }
}