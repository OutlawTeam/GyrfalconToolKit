using Gyrfalcon.Engine.Module.Luxon.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GyrfalconToolKit.Editor.Utils
{
    internal static class GridRenderer
    {
        static int VAO;
        static int VBO;
        static int GridVAO;
        static int GridVBO;
        static string VertShader = @"
                    #version 330 core
                    layout (location = 0) in vec3 aPosition;
                    uniform mat4 model;
                    uniform mat4 view;
                    uniform mat4 projection;
                    void main()
                    {
                        gl_Position = vec4(aPosition, 1.0) * model * view * projection;
                    }
                    ";

        static string FragShader = @"
                    #version 330 core
                    out vec4 FragColor;

                    void main()
                    {
                        FragColor = vec4(1, 0,0, 1.0);
                    }
                    ";
        static string GridVertShader = @"
                    #version 330 core
                    layout (location = 0) in vec3 aPosition;
                    uniform mat4 view;
                    uniform mat4 projection;
                    out vec3 NearPoint;
                    out vec3 FarPoint;
                    out mat4 fragView;
                    out mat4 fragProj;
                    vec3 UnprojectPoint(float x, float y, float z, mat4 view, mat4 projection) {
                        mat4 viewInv = inverse(view);
                        mat4 projInv = inverse(projection);
                        vec4 unprojectedPoint =    vec4(x, y, z, 1.0) * projInv*viewInv  ;
                        return unprojectedPoint.xyz / unprojectedPoint.w;
                    }
                    void main()
                    {
                        vec3 p = aPosition;
                        NearPoint = UnprojectPoint(p.x, p.y, 0.0, view, projection).xyz; // unprojecting on the near plane
                        FarPoint = UnprojectPoint(p.x, p.y, 1.0, view, projection).xyz; // unprojecting on the far plane
                        gl_Position = vec4(p, 1.0); // using directly the clipped coordinates
                        fragView = view;
                        fragProj = projection;
                    }
                    ";
        static string GridFragShader = @"
                    #version 330 core
                    out vec4 FragColor;
                    in vec3 NearPoint;
                    in vec3 FarPoint;
                    in mat4 fragView;
                    in mat4 fragProj;
                    vec4 grid(vec3 fragPos3D, float scale,float width, bool drawAxis) {
                        vec2 coord = fragPos3D.xz * scale; // use the scale variable to set the distance between the lines
                        vec2 derivative = fwidth(coord) *width;
                        vec2 grid = abs(fract(coord - 0.5) - 0.5) / derivative;
                        float line = min(grid.x, grid.y);
                        float minimumz = min(derivative.y, 1);
                        float minimumx = min(derivative.x, 1);
                        vec4 color = vec4(0.329412, 0.329412, 0.329412, 1.0 - min(line, 1.0));
                        if(drawAxis)
                        {
                            // z axis
                            if(fragPos3D.x > -0.2 * minimumx && fragPos3D.x < 0.2 * minimumx)
                            {
                                color.z = 1.0;
                                color.x = 0;
                                color.y = 0;
                            }
                            // x axis
                            if(fragPos3D.z > -0.2 * minimumz && fragPos3D.z < 0.2 * minimumz)
                            {
                                color.x = 1.0;
                                color.y = 0;
                                color.z = 0;
                            }
                                
                        }
                        return color;
                    }
                    float computeDepth(vec3 pos) {
                        vec4 clip_space_pos = vec4(pos.xyz, 1.0)*fragView*  fragProj;
                        return (clip_space_pos.z / clip_space_pos.w);
                    }
                    float computeLinearDepth(vec3 pos) {
                        vec4 clip_space_pos = vec4(pos.xyz, 1.0)* fragView *  fragProj  ;
                        float clip_space_depth = (clip_space_pos.z / clip_space_pos.w) * 2.0 - 1.0; // put back between -1 and 1
                        float linearDepth = (2.0 *0.01 * 100) / (100 + 0.01 - clip_space_depth * (100 - 0.01)); // get linear value between 0.01 and 100
                        return linearDepth / 100; // normalize
                    }
                    void main()
                    {
                        float t = -NearPoint.y / (FarPoint.y - NearPoint.y);
                        vec3 fragPos3D = NearPoint + t * (FarPoint - NearPoint);
                        //gl_FragDepth = computeDepth(fragPos3D);
                        float linearDepth = computeLinearDepth(fragPos3D);
                        float fading = max(0, (0.5 - linearDepth));
                        FragColor =(grid(fragPos3D, 1,2,true) /*+ grid(fragPos3D, 10,1,false)*/) * float(t > 0); // opacity = 1 when t > 0, opacity = 0 otherwise
                        FragColor.r = clamp(FragColor.r, 0.0, 1.0);
                        FragColor.g = clamp(FragColor.g, 0.0, 1.0);
                        FragColor.b = clamp(FragColor.b, 0.0, 1.0);

//FragColor.a *= fading;
                    }
                    ";
        private static Shader shader = new Shader(VertShader, FragShader, true);
        private static Shader GridShader = new Shader(GridVertShader, GridFragShader, true);
        internal static void GenerateGridVertices()
        {
            /*
            float[] vertices = new float[80 * 3]; // 80 points, 3 coordonnées par point (x, y, z)

            int index = 0;
            for (float i = -5.0f; i <= 5.0f; i += 1.0f)
            {
                // Lignes horizontales
                vertices[index++] = -5.0f;
                vertices[index++] = 0.0f; // Y reste constant pour les lignes horizontales
                vertices[index++] = i;

                vertices[index++] = 5.0f;
                vertices[index++] = 0.0f;
                vertices[index++] = i;

                // Lignes verticales
                vertices[index++] = i;
                vertices[index++] = 0.0f;
                vertices[index++] = -5.0f;

                vertices[index++] = i;
                vertices[index++] = 0.0f;
                vertices[index++] = 5.0f;
            }

            // Création du Vertex Array Object (VAO)
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            // Création du Vertex Buffer Object (VBO)
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Configuration de l'attribut de position
            int positionLocation = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);*/
            float[] vertices = new float[]
            {
                 1, 1, 0, -1, -1, 0, -1, 1, 0,
    -1, -1, 0, 1, 1, 0, 1, -1, 0
            };
            // Création du Vertex Array Object (VAO)
            GridVAO = GL.GenVertexArray();
            GL.BindVertexArray(GridVAO);

            // Création du Vertex Buffer Object (VBO)
            GridVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, GridVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Configuration de l'attribut de position
            int positionLocation = GridShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        internal static void Render(Matrix4 View, Matrix4 Projection)
        {
            /*
            shader.Use();
            shader.SetMatrix4("view", View);
            shader.SetMatrix4("projection", Projection);
            shader.SetMatrix4("model", Matrix4.CreateTranslation(0, 0, 0));
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(16.0f);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Lines, 0, 80);
            GL.BindVertexArray(0);
            GL.Disable(EnableCap.LineSmooth);*/
            GL.Disable(EnableCap.CullFace);
            GridShader.Use();
            GridShader.SetMatrix4A("view", Matrix4.Transpose(View));
            GridShader.SetMatrix4A("projection", Matrix4.Transpose(Projection));
            GL.BindVertexArray(GridVAO);
            GL.Disable(EnableCap.DepthTest);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

        }
    }
}
