using UnityEngine;
using UnityEngine.UI;

public class GearPreviewClick : MonoBehaviour
{
    [Header("渲染相机与贴图")]
    public RenderTexture gearRT;
    public Camera renderCam;
    public GearLockManager lockMgr;

    private RawImage rawImg;
    private Transform dragGear;
    private Vector2 startUV;
    private float startGearZ;

    void Awake()
    {
        rawImg = GetComponent<RawImage>();

    }

    void Update()
    {
        // 鼠标按下开始拖拽
        if (Input.GetMouseButtonDown(0))
        {
            RayCastGear(out dragGear, out startUV);
            if (dragGear != null)
            {
                startGearZ = dragGear.localEulerAngles.z;
            }
        }

        // 拖拽中实时旋转
        if (Input.GetMouseButton(0) && dragGear != null)
        {
            Vector2 currentUV;
            if (GetMouseUV(out currentUV))
            {
                float delta = (currentUV.x - startUV.x) * 360f;
                float newZ = startGearZ + delta;
                dragGear.localEulerAngles = new Vector3(0, 0, newZ);

                // 修复：GameObject点transform
                int index = dragGear.GetComponent<UIGearClick>().GearIndex;
                lockMgr.gearObjs[index].transform.localEulerAngles = new Vector3(0, 0, newZ);

                // 角度转数字实时更新密码
                lockMgr.gearNum[index] = AngleToNumber(newZ);
            }
        }

        // 松开清空拖拽对象
        if (Input.GetMouseButtonUp(0))
        {
            dragGear = null;
        }
    }

    bool RayCastGear(out Transform gearTrans, out Vector2 uvOut)
    {
        gearTrans = null;
        uvOut = Vector2.zero;

        Rect rectArea = rawImg.rectTransform.rect;
        Vector2 mouseLocalPos;
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImg.rectTransform, Input.mousePosition, null, out mouseLocalPos);
        if (!inside) return false;

        Vector2 uv;
        uv.x = Mathf.InverseLerp(rectArea.xMin, rectArea.xMax, mouseLocalPos.x);
        uv.y = Mathf.InverseLerp(rectArea.yMin, rectArea.yMax, mouseLocalPos.y);
        uvOut = uv;

        Ray ray = renderCam.ViewportPointToRay(uv);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            UIGearClick gear = hit.collider.GetComponent<UIGearClick>();
            if (gear != null)
            {
                gearTrans = hit.transform;
                return true;
            }
        }
        return false;
    }

    bool GetMouseUV(out Vector2 uvOut)
    {
        uvOut = Vector2.zero;
        Rect rectArea = rawImg.rectTransform.rect;
        Vector2 mouseLocalPos;
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImg.rectTransform, Input.mousePosition, null, out mouseLocalPos);
        if (!inside) return false;

        Vector2 uv;
        uv.x = Mathf.InverseLerp(rectArea.xMin, rectArea.xMax, mouseLocalPos.x);
        uv.y = Mathf.InverseLerp(rectArea.yMin, rectArea.yMax, mouseLocalPos.y);
        uvOut = uv;
        return true;
    }

    /// 角度分区映射数字
    int AngleToNumber(float zAngle)
    {
        float angle = zAngle % 360f;
        if (angle < 0) angle += 360f;

        if ((angle >= 342 && angle <= 360) || (angle >= 0 && angle <= 18))
            return 0;
        else if (angle > 18 && angle <= 54)
            return 1;
        else if (angle > 54 && angle <= 90)
            return 2;
        else if (angle > 90 && angle <= 126)
            return 3;
        else if (angle > 126 && angle <= 162)
            return 4;
        else if (angle > 162 && angle <= 198)
            return 5;
        else if (angle > 198 && angle <= 234)
            return 6;
        else if (angle > 234 && angle <= 270)
            return 7;
        else if (angle > 270 && angle <= 306)
            return 8;
        else
            return 9;
    }
}